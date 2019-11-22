using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class PurchaseRequestAttachmentService : IPurchaseRequestAttachmentService
    {
        private readonly IObjectStorageService _objectStorageService;
        private readonly ApplicationDbContext _db;

        public PurchaseRequestAttachmentService(
            IObjectStorageService objectStorageService,
            ApplicationDbContext db)
        {
            _objectStorageService = objectStorageService;
            _db = db;
        }

        public async Task SaveAttachmentAsync(Guid purchaseRequestId, Stream stream, string fileName)
        {
            var pra = new PurchaseRequestAttachment
            {
                PurchaseRequestId = purchaseRequestId,
                FileName = fileName
            };

            var praEntry = await _db.PurchaseRequestAttachments.AddAsync(pra);
            await _db.SaveChangesAsync();

            var path = praEntry.Entity.BuildPath();
            await _objectStorageService.SaveFileAsync(path, stream);
        }

        public async Task<IEnumerable<PurchaseRequestAttachmentDto>> GetAttachmentsAsync(Guid purchaseRequestId)
        {
            var attachments = await _db.PurchaseRequestAttachments
                .Where(q => q.PurchaseRequestId == purchaseRequestId)
                .ProjectToType<PurchaseRequestAttachmentDto>()
                .ToListAsync();

            return attachments;
        }

        public async Task<IEnumerable<string>> GetAttachmentsPathAsync(Guid purchaseRequestId)
        {
            var attachments = await _db.PurchaseRequestAttachments
                .Where(q => q.PurchaseRequestId == purchaseRequestId)
                .ToListAsync();

            var result = new List<string>();

            foreach(var attachment in attachments)
            {
                var path = attachment.BuildPath();
                var stream = await _objectStorageService.GetFileStreamAsync(path);
                var tempFilePath = Path.Combine(Path.GetTempPath(), attachment.FileName);
                using (var fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fs);
                }
                result.Add(tempFilePath);
            }

            return result;
        }

        public async Task DeleteAttachment(Guid attachmentId)
        {
            var attachment = _db.PurchaseRequestAttachments.Find(attachmentId);
            var path = attachment.BuildPath();
            await _objectStorageService.DeleteFileAsync(path);
            _db.PurchaseRequestAttachments.Remove(attachment);
            await _db.SaveChangesAsync();
        }
    }
}

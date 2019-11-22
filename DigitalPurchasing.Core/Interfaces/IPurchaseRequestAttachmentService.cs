using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IPurchaseRequestAttachmentService
    {
        Task SaveAttachmentAsync(Guid purchaseRequestId, Stream stream, string fileName);
        Task<IEnumerable<PurchaseRequestAttachmentDto>> GetAttachmentsAsync(Guid purchaseRequestId);
        Task DeleteAttachment(Guid attachmentId);
        Task<IEnumerable<string>> GetAttachmentsPathAsync(Guid purchaseRequestId);
    }

    public class PurchaseRequestAttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
    }
}

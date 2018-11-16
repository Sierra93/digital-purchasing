using System;
using System.Linq;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;

namespace DigitalPurchasing.Services
{
    public class UploadedDocumentService : IUploadedDocumentService
    {
        private readonly ApplicationDbContext _db;

        public UploadedDocumentService(ApplicationDbContext db) => _db = db;

        public DeleteResultVm Delete(Guid id)
        {
            var doc = _db.UploadedDocuments.Find(id);
            if (doc != null)
            {
                var pr = _db.PurchaseRequests.FirstOrDefault(q => q.UploadedDocumentId == id);
                if (pr != null)
                {
                    pr.UploadedDocumentId = null;
                }
                var so = _db.SupplierOffers.FirstOrDefault(q => q.UploadedDocumentId == id);
                if (so != null)
                {
                    so.UploadedDocumentId = null;
                }
                _db.UploadedDocumentHeaders.Remove(_db.UploadedDocumentHeaders.Find(doc.UploadedDocumentHeadersId));
                _db.UploadedDocuments.Remove(doc);
                _db.SaveChanges();
            }
            return DeleteResultVm.Success();
        }
    }
}

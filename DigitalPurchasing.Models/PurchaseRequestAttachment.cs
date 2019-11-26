using System;

namespace DigitalPurchasing.Models
{
    public class PurchaseRequestAttachment : BaseModel
    {
        public Guid PurchaseRequestId { get; set; }
        public PurchaseRequest PurchaseRequest { get; set; }
        public string FileName { get; set; }

        public string BuildPath()
        {
            var extension = System.IO.Path.GetExtension(FileName);
            var path = $"{PurchaseRequestId:N}/{Id:N}{extension}";
            return path;
        }
    }
}

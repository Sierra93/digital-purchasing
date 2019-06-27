using System;
using DigitalPurchasing.Core.Interfaces;
using System.Collections.Generic;
using DigitalPurchasing.Core.Enums;

namespace DigitalPurchasing.Models
{
    public class PurchaseRequest : BaseModelWithOwner
    {
        public Guid? CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int PublicId { get; set; }
        public string ErpCode { get; set; }

        public string CompanyName { get; set; }
        public string CustomerName { get; set; }

        public PurchaseRequestStatus Status { get; set; }

        public ICollection<PurchaseRequestItem> Items { get; set; } = new List<PurchaseRequestItem>();

        public QuotationRequest QuotationRequest { get; set; }

        public Guid? UploadedDocumentId { get; set; }
        public UploadedDocument UploadedDocument { get; set; }

        public Delivery Delivery { get; set; }
        public Guid? DeliveryId { get; set; }
    }
}

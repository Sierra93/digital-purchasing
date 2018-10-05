using System;

namespace DigitalPurchasing.Models
{
    public class QuotationRequest : BaseModelWithOwner
    {
        public int PublicId { get; set; }

        public PurchaseRequest PurchaseRequest { get; set; }
        public Guid PurchaseRequestId { get; set; }

        public Delivery Delivery { get; set; }
        public Guid? DeliveryId { get; set; }
    }
}

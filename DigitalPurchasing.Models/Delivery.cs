using System;
using System.Collections.Generic;
using System.Text;
using DigitalPurchasing.Core;

namespace DigitalPurchasing.Models
{
    public class Delivery : BaseModelWithOwner
    {
        public DateTime DeliverAt { get; set; }
        public DeliveryTerms DeliveryTerms { get; set; }
        public PaymentTerms PaymentTerms { get; set; }

        public int PayWithinDays { get; set; }

        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }

        public string House { get; set; }
        public string Building { get; set; }
        public string Structure { get; set; }
        public string OfficeOrApartment { get; set; }

        public ICollection<PurchaseRequest> PurchaseRequests { get; set; }
        public ICollection<QuotationRequest> QuotationRequests { get; set; }
    }
}

using System;
using DigitalPurchasing.Models.Identity;

namespace DigitalPurchasing.Models
{
    public class QuotationRequestEmail : BaseModel
    {
        public Guid RequestId { get; set; }
        public QuotationRequest Request { get; set; }

        public Guid ContactPersonId { get; set; }
        public SupplierContactPerson ContactPerson { get; set; }

        public bool ByCategory { get; set; }
        public bool ByItem { get; set; }

        public string Data { get; set; }

        public User User { get; set; }
        public Guid? UserId { get; set; }
    }
}

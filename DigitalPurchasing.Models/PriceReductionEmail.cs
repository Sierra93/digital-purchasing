using System;
using DigitalPurchasing.Models.Identity;

namespace DigitalPurchasing.Models
{
    public class PriceReductionEmail : BaseModel
    {
        public Guid ContactPersonId { get; set; }
        public SupplierContactPerson ContactPerson { get; set; }

        public Guid SupplierOfferId { get; set; }
        public SupplierOffer SupplierOffer { get; set; }

        public Guid? UserId { get; set; }
        public User User { get; set; }

        public string Data { get; set; }
    }
}

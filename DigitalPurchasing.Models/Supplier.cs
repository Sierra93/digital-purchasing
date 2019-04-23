using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public class Supplier : BaseModelWithOwner
    {
        public string Name { get; set; }
        public string OwnershipType { get; set; }
        public long? Inn { get; set; }
        public string ErpCode { get; set; }
        public string Website { get; set; }
        public string LegalAddressStreet { get; set; }
        public string LegalAddressCity { get; set; }
        public string LegalAddressCountry { get; set; }
        public string ActualAddressStreet { get; set; }
        public string ActualAddressCity { get; set; }
        public string ActualAddressCountry { get; set; }
        public string WarehouseAddressStreet { get; set; }
        public string WarehouseAddressCity { get; set; }
        public string WarehouseAddressCountry { get; set; }

        public ICollection<SupplierOffer> Offers { get; set; }

        public ICollection<SupplierContactPerson> ContactPersons { get; set; }
    }
}

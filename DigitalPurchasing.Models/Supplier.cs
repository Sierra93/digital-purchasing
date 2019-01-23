using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public class Supplier : BaseModelWithOwner
    {
        public string Name { get; set; }

        public ICollection<SupplierOffer> SupplierOffers { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public class Customer : BaseModelWithOwner
    {
        public string Name { get; set; }
        public int PublicId { get; set; }

        public ICollection<PurchaseRequest> Requests { get; set; }
    }
}

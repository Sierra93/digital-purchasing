using DigitalPurchasing.Core.Interfaces;
using System.Collections.Generic;

namespace DigitalPurchasing.Models
{
    public class PurchasingRequest : BaseModelWithOwner
    {
        public int PublicId { get; set; }

        public string CompanyName { get; set; }
        public string CustomerName { get; set; }

        public PurchasingRequestStatus Status { get; set; }

        public ICollection<PurchasingRequestItem> Items { get; set; } = new List<PurchasingRequestItem>();

        public string RawData { get; set; }

        public RawColumns RawColumns { get; set; }
    }
}

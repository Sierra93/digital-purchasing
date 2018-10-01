using DigitalPurchasing.Core.Interfaces;
using System.Collections.Generic;

namespace DigitalPurchasing.Models
{
    public class PurchaseRequest : BaseModelWithOwner
    {
        public int PublicId { get; set; }

        public string CompanyName { get; set; }
        public string CustomerName { get; set; }

        public PurchaseRequestStatus Status { get; set; }

        public ICollection<PurchaseRequestItem> Items { get; set; } = new List<PurchaseRequestItem>();

        public string RawData { get; set; }

        public RawColumns RawColumns { get; set; }
    }
}

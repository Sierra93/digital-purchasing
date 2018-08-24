using DigitalPurchasing.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public class PurchasingRequest : BaseModelWithOwner
    {
        public PurchasingRequestType Type { get; set; }

        public ICollection<PurchasingRequestItem> Items { get; set; } = new List<PurchasingRequestItem>();
        public ICollection<RawPurchasingRequestItem> RawItems { get; set; } = new List<RawPurchasingRequestItem>();

        public string RawData { get; set; }
    }

    public class PurchasingRequestItem : BaseModel
    {
        public PurchasingRequest PurchasingRequest { get; set; }
        public Guid PurchasingRequestId { get; set; }
    }

    public class RawPurchasingRequestItem : BaseModel
    {
        public PurchasingRequest PurchasingRequest { get; set; }
        public Guid PurchasingRequestId { get; set; }

        public int Position { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Uom { get; set; }
        public decimal Qty { get; set; }
    }
}

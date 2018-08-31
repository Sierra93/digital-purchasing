using DigitalPurchasing.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigitalPurchasing.Models
{
    public class PurchasingRequest : BaseModelWithOwner
    {
        public int PublicId { get; set; }

        public PurchasingRequestStatus Status { get; set; }

        public ICollection<PurchasingRequestItem> Items { get; set; } = new List<PurchasingRequestItem>();
        public ICollection<RawPurchasingRequestItem> RawItems { get; set; } = new List<RawPurchasingRequestItem>();

        public string RawData { get; set; }

        public RawColumns RawColumns { get; set; }
    }

    public class RawColumns
    {
        [Key]
        public Guid RawColumnsId { get; set; } = Guid.NewGuid();

        // columns
        public string Code { get; set; }
        public string Name { get; set; }
        public string Uom { get; set; }
        public string Qty { get; set; }
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

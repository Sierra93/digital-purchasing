using System;

namespace DigitalPurchasing.Models.SSR
{
    public class SSSupplierItem : SSBase
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }

        public SSSupplier Supplier { get; set; }
        public Guid SupplierId { get; set; }

        public Guid InternalId { get; set; }

        public Guid NomenclatureId { get; set; }
    }
}

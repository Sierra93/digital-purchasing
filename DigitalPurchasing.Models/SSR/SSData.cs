using System;

namespace DigitalPurchasing.Models.SSR
{
    public class SSData : SSBase
    {
        public SSVariant Variant { get; set; }
        public Guid VariantId { get; set; }

        public SSSupplier Supplier { get; set; }
        public Guid SupplierId { get; set; }

        public Guid NomenclatureId { get; set; }

        public decimal Quantity { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalPurchasing.Models.SSR
{
    public class SSData : SSBase
    {
        public SSVariant Variant { get; set; }
        public Guid VariantId { get; set; }

        public SSSupplier Supplier { get; set; }
        public Guid SupplierId { get; set; }

        public Guid NomenclatureId { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(38, 17)")]
        public decimal Price { get; set; }
    }
}

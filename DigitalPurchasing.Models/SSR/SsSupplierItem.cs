using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalPurchasing.Models.SSR
{
    public class SSSupplierItem : SSBase
    {
        public string Name { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(38, 17)")]
        public decimal Price { get; set; }

        public SSSupplier Supplier { get; set; }
        public Guid SupplierId { get; set; }

        public Guid InternalId { get; set; }

        public Guid NomenclatureId { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal ConvertedQuantity { get; set; }

        [Column(TypeName = "decimal(38, 17)")]
        public decimal ConvertedPrice { get; set; }

        public string UomStr { get; set; }

        public string OfferInvoiceData { get; set; }
    }
}

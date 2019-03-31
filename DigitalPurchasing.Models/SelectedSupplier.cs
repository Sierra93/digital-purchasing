using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalPurchasing.Models
{
    public class SelectedSupplier : BaseModelWithOwner
    {
        public Root Root { get; set; }
        public Guid RootId { get; set; }

        public Supplier Supplier { get; set; }
        public Guid SupplierId { get; set; }

        public Nomenclature Nomenclature { get; set; }
        public Guid NomenclatureId { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Qty { get; set; }
    }
}

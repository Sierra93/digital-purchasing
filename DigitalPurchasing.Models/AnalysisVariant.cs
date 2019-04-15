using System;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalPurchasing.Analysis2.Enums;
using DigitalPurchasing.Core;

namespace DigitalPurchasing.Models
{
    public class AnalysisVariant : BaseModelWithOwner
    {
        public CompetitionList CompetitionList { get; set; }
        public Guid? CompetitionListId { get; set; }

        public PaymentTerms PaymentTerms { get; set; }
        public DeliveryDateTerms DeliveryDateTerms { get; set; }
        public DeliveryTerms DeliveryTerms { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalValue { get; set; }
        
        public int SupplierCount { get; set; }
        public SupplierCountType SupplierCountType { get; set; }

        public bool IsSelected { get; set; }
    }

    public class AnalysisResultItem : BaseModelWithOwner
    {
        public AnalysisVariant Variant { get; set; }
        public Guid VariantId { get; set; }

        public Supplier Supplier { get; set; }
        public Guid SupplierId { get; set; }

        public Nomenclature Nomenclature { get; set; }
        public Guid NomenclatureId { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Quantity { get; set; }
    }
}

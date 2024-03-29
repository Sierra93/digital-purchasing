using System;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Enums;

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
}

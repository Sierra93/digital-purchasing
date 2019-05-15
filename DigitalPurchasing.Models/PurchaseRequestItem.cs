using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalPurchasing.Models
{
    public class PurchaseRequestItem : BaseModel
    {
        public PurchaseRequest PurchaseRequest { get; set; }
        public Guid PurchaseRequestId { get; set; }

        public Nomenclature Nomenclature { get; set; }
        public Guid? NomenclatureId { get; set; }
        
        public int Position { get; set; }
        public string RawCode { get; set; }
        public string RawName { get; set; }
        public string RawUom { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal RawQty { get; set; }

        public Guid? RawUomMatchId { get; set; }
        public UnitsOfMeasurement RawUomMatch { get; set; }

        [Column(TypeName = "decimal(38, 17)")]
        public decimal CommonFactor { get; set; }
        [Column(TypeName = "decimal(38, 17)")]
        public decimal NomenclatureFactor { get; set; }
    }
}

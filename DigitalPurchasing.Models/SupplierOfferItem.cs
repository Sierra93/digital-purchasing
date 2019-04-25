using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalPurchasing.Models
{
    public class SupplierOfferItem : BaseModel
    {
        public SupplierOffer SupplierOffer { get; set; }
        public Guid SupplierOfferId { get; set; }
        
        public int Position { get; set; }
        public string RawCode { get; set; }
        public string RawName { get; set; }
        public string RawUomStr { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal RawQty { get; set; }
        [Column(TypeName = "decimal(18, 4)")] 
        public decimal RawPrice { get; set; }

        public Guid? RawUomId { get; set; }
        public UnitsOfMeasurement RawUom { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal CommonFactor { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal NomenclatureFactor { get; set; }

        public Nomenclature Nomenclature { get; set; }
        public Guid? NomenclatureId { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalPurchasing.Core;

namespace DigitalPurchasing.Models
{
    public class NomenclatureAlternative : BaseModelWithOwner
    {
        public NomenclatureAlternativeLink Link { get; set; }

        public Guid NomenclatureId { get; set; }
        public Nomenclature Nomenclature { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

        public Guid? BatchUomId { get; set; }
        public UnitsOfMeasurement BatchUom { get; set; }

        public Guid? MassUomId { get; set; }
        public UnitsOfMeasurement MassUom { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal MassUomValue { get; set; }
       
        public Guid? ResourceUomId { get; set; }
        public UnitsOfMeasurement ResourceUom { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal ResourceUomValue { get; set; }

        public Guid? ResourceBatchUomId { get; set; }
        public UnitsOfMeasurement ResourceBatchUom { get; set; }

        public Guid? PackUomId { get; set; }
        public UnitsOfMeasurement PackUom { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal? PackUomValue { get; set; }
    }
}

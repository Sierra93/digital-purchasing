using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalPurchasing.Models
{
    public class Nomenclature : BaseModelWithOwner
    {
        public string Code { get; set; }

        public string Name { get; set; }
        public string NameEng { get; set; }

        public Guid BatchUomId { get; set; }
        public UnitsOfMeasurement BatchUom { get; set; }

        public Guid MassUomId { get; set; }
        public UnitsOfMeasurement MassUom { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal MassUomValue { get; set; }

        public Guid ResourceUomId { get; set; }
        public UnitsOfMeasurement ResourceUom { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal ResourceUomValue { get; set; }

        public Guid ResourceBatchUomId { get; set; }
        public UnitsOfMeasurement ResourceBatchUom { get; set; }

        public Guid CategoryId { get; set; }
        public NomenclatureCategory Category { get; set; }

        public Guid? PackUomId { get; set; }
        public UnitsOfMeasurement PackUom { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal PackUomValue { get; set; }

        public bool IsDeleted { get; set; }

        public List<NomenclatureAlternative> Alternatives { get; set; }

        public List<NomenclatureComparisonData> ComparisonDataItems { get; set; } = new List<NomenclatureComparisonData>();

        public List<NomenclatureComparisonDataNGram> Ngrams { get; set; } = new List<NomenclatureComparisonDataNGram>();
    }
}

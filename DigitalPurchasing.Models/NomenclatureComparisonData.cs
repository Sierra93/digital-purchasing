using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public class NomenclatureComparisonData : BaseModel
    {
        public string AdjustedNomenclatureName { get; set; }
        public string NomenclatureDimensions { get; set; }
        public string AdjustedNomenclatureNameWithDimensions { get; set; }
        public string AdjustedNomenclatureDigits { get; set; }
        public Guid NomenclatureId { get; set; }
        public Guid? NomenclatureAlternativeId { get; set; }
    }
}

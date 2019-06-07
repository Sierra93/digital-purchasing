using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public sealed class NomenclatureComparisonDataNGram : AppNGram
    {
        public Guid NomenclatureComparisonDataId { get; set; }
        public NomenclatureComparisonData NomenclatureComparisonData { get; set; }
        public Guid OwnerId { get; set; }

        public Guid NomenclatureId { get; set; }
        public Nomenclature Nomenclature { get; set; }

        public Guid? NomenclatureAlternativeId { get; set; }
        public NomenclatureAlternative NomenclatureAlternative { get; set; }
    }
}

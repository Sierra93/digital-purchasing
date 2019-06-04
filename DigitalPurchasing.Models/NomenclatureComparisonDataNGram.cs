using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public sealed class NomenclatureComparisonDataNGram : AppNGram
    {
        public Guid NomenclatureComparisonDataId { get; set; }
        public NomenclatureComparisonData NomenclatureComparisonData { get; set; }
    }
}

using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core.Interfaces.Analysis;

namespace DigitalPurchasing.Analysis.Filters
{
    public class VariantsItemMustHavePriceFilter : IFilter<IEnumerable<IEnumerable<AnalysisResultData>>>
    {
        public IEnumerable<IEnumerable<AnalysisResultData>> Execute(IEnumerable<IEnumerable<AnalysisResultData>> input)
            => input.Where(q => q.All(w => w.TotalPrice > 0));
    }
}

using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core.Interfaces.Analysis;
using DigitalPurchasing.Core.Interfaces.Analysis.VariantOptions;

namespace DigitalPurchasing.Analysis.Filters
{
    public class VariantsTotalValueFilter : IFilter<IEnumerable<IEnumerable<AnalysisResultData>>>
    {
        private readonly decimal _totalValue;

        public VariantsTotalValueFilter(VariantsTotalValueOptions options)
            => _totalValue = options.Value;

        public IEnumerable<IEnumerable<AnalysisResultData>> Execute(IEnumerable<IEnumerable<AnalysisResultData>> input)
            => _totalValue <= 0
                ? input
                : input.Where(q => q.Sum(w => w.TotalPrice) <= _totalValue);
    }
}

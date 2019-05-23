using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core.Interfaces.Analysis;

namespace DigitalPurchasing.Analysis.Filters
{
    public class VariantsItemQuantityFilter : IFilter<IEnumerable<IEnumerable<AnalysisResultData>>>
    {
        public IEnumerable<IEnumerable<AnalysisResultData>> Execute(IEnumerable<IEnumerable<AnalysisResultData>> input)
            => input
                .Where(w => w.GroupBy(g => g.NomenclatureId)
                .Select(q => new {
                    NomenclatureId = q.Key,
                    Qty = q.Sum(e => e.Quantity),
                    CustomerQuantity = q.First().CustomerQuantity
                })
                .All(r => r.Qty >= r.CustomerQuantity));
    }
}

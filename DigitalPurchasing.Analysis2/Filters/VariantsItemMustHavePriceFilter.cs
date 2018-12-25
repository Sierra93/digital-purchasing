using System.Collections.Generic;
using System.Linq;

namespace DigitalPurchasing.Analysis2.Filters
{
    public class VariantsItemMustHavePriceFilter : VariantsFilter
    {
        public override List<List<AnalysisData>> Filter(List<List<AnalysisData>> allVariants, IAnalysisContext context)
            => allVariants.Where(q => q.All(w => w.Item.TotalPrice > 0)).ToList();
    }
}

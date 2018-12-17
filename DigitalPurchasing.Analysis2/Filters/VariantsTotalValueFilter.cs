using System.Collections.Generic;
using System.Linq;

namespace DigitalPurchasing.Analysis2.Filters
{
    public class VariantsTotalValueFilter : VariantsFilter<VariantsTotalValueOptions>
    {
        public VariantsTotalValueFilter(VariantsTotalValueOptions options) => Options = options;

        public override List<List<AnalysisData>> Filter(List<List<AnalysisData>> allVariants, IAnalysisContext context)
        {
            if (Options == null || Options.Value <= 0) return allVariants;

            return allVariants
                .Where(q => q.Sum(w => w.Item.TotalPrice) <= Options.Value)
                .ToList();
        }
    }
}

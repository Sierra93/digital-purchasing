using System.Collections.Generic;

namespace DigitalPurchasing.Analysis2.Filters
{
    public abstract class VariantsFilter<TOptions> : BaseFilterOptions<TOptions>, IVariantsFilter where TOptions : class, new()
    {
        public abstract List<List<AnalysisData>> Filter(List<List<AnalysisData>> allVariants, IAnalysisContext context);
    }
}

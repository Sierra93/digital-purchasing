using System.Collections.Generic;

namespace DigitalPurchasing.Analysis2.Filters
{
    public abstract class SuppliersFilter<TOptions> : BaseFilterOptions<TOptions>, ISuppliersFilter where TOptions : class, new()
    {
        public abstract List<AnalysisSupplier> Filter(List<AnalysisSupplier> suppliers, IAnalysisContext context);
    }
}

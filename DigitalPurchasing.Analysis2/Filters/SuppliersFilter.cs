using System.Collections.Generic;

namespace DigitalPurchasing.Analysis2.Filters
{
    public abstract class SuppliersFilter<TOptions> : BaseFilterOptions<TOptions>, ISuppliersFilter where TOptions : class, new()
    {
        public abstract List<Supplier> Filter(List<Supplier> suppliers, IAnalysisContext context);
    }
}

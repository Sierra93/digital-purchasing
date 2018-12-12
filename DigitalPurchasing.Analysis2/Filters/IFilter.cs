using System.Collections.Generic;

namespace DigitalPurchasing.Analysis2.Filters
{
    public interface IFilter<T>
    {
        List<T> Filter(List<T> input, IAnalysisContext context);
    }
}

using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Interfaces.Analysis;
using DigitalPurchasing.Core.Interfaces.Analysis.VariantOptions;

namespace DigitalPurchasing.Analysis.Filters
{
    public class VariantsSuppliersCountFilter : IFilter<IEnumerable<IEnumerable<AnalysisResultData>>>
    {
        private readonly SupplierCountType _type;
        private readonly int _count;

        public VariantsSuppliersCountFilter(VariantsSuppliersCountOptions options)
        {
            _type = options.Type;
            _count = options.Count;
        }

        public IEnumerable<IEnumerable<AnalysisResultData>> Execute(IEnumerable<IEnumerable<AnalysisResultData>> input)
        {
            switch (_type)
            {
                case SupplierCountType.Equal:
                    return input.Where(q => q.Select(w => w.SupplierId).Distinct().Count() == _count);
                case SupplierCountType.LessOrEqual:
                    return input.Where(q => q.Select(w => w.SupplierId).Distinct().Count() <= _count);
                case SupplierCountType.Any:
                    return input;
                default:
                    return input;
            }
        }
    }
}

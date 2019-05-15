using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Analysis2.Enums;

namespace DigitalPurchasing.Analysis2.Filters
{
    public class VariantsSuppliersCountFilter : VariantsFilter<VariantsSuppliersCountOptions>
    {
        public override int Order => 0;

        public VariantsSuppliersCountFilter(VariantsSuppliersCountOptions options) => Options = options;

        public override List<List<AnalysisData>> Filter(List<List<AnalysisData>> allVariants, IAnalysisContext context)
        {
            if (Options?.Type != SupplierCountType.Any)
            {
                switch (Options?.Type)
                {
                    case SupplierCountType.Equal:
                        allVariants = allVariants
                            .Where(q => q.Select(w => w.SupplierId).Distinct().Count() == Options.Count)
                            .ToList();
                        return allVariants;
                    case SupplierCountType.LessOrEqual:
                        allVariants = allVariants
                            .Where(q => q.Select(w => w.SupplierId).Distinct().Count() <= Options.Count)
                            .ToList();
                        return allVariants;
                }
            }

            return allVariants;
        }
    }
}

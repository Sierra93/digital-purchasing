using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core;

namespace DigitalPurchasing.Analysis2.Filters
{
    public class SupplierDeliveryTermsFilter : SuppliersFilter<SupplierDeliveryTermsOptions>
    {
        public SupplierDeliveryTermsFilter(SupplierDeliveryTermsOptions options) => Options = options;

        public override List<AnalysisSupplier> Filter(List<AnalysisSupplier> suppliers, IAnalysisContext context)
        {
            if (Options != null && Options.DeliveryTerms != DeliveryTerms.NoRequirements)
            {
                return suppliers.Where(q => q.DeliveryTerms == Options.DeliveryTerms).ToList();
            }

            return suppliers;
        }
    }
}

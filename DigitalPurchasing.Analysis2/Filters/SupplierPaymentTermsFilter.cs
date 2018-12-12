using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core;

namespace DigitalPurchasing.Analysis2.Filters
{
    public class SupplierPaymentTermsFilter : SuppliersFilter<SupplierPaymentTermsOptions>
    {
        public SupplierPaymentTermsFilter(SupplierPaymentTermsOptions options) => Options = options;

        public override List<Supplier> Filter(List<Supplier> suppliers, IAnalysisContext context)
        {
            if (Options != null && Options.PaymentTerms != PaymentTerms.NoRequirements)
            {
                return suppliers.Where(q => q.PaymentTerms == Options.PaymentTerms).ToList();
            }

            return suppliers;
        }
    }
}

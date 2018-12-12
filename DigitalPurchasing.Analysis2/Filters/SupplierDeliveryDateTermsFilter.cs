using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Analysis2.Enums;

namespace DigitalPurchasing.Analysis2.Filters
{
    public class SupplierDeliveryDateTermsFilter : SuppliersFilter<SupplierDeliveryDateTermsOptions>
    {
        public SupplierDeliveryDateTermsFilter(SupplierDeliveryDateTermsOptions options) => Options = options;

        public override List<Supplier> Filter(List<Supplier> suppliers, IAnalysisContext context)
        {
            if (Options != null && Options.DeliveryDateTerms != DeliveryDateTerms.Any)
            {
                if (Options.DeliveryDateTerms == DeliveryDateTerms.Min)
                {
                    var minDate = suppliers.Where(q => q.Date.HasValue).Select(q => q.Date.Value).Min();
                    suppliers = suppliers.Where(q => q.Date.HasValue && q.Date == minDate).ToList();
                }
                else if (Options.DeliveryDateTerms == DeliveryDateTerms.LessThanInRequest)
                {
                    suppliers = suppliers.Where(q => q.Date.HasValue && q.Date <= context.Customer.Date).ToList();
                }
            }

            return suppliers;
        }
    }
}
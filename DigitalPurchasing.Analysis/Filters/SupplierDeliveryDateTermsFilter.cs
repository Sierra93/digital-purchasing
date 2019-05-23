using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Interfaces.Analysis.VariantOptions;

namespace DigitalPurchasing.Analysis.Filters
{
    public class SupplierDeliveryDateTermsFilter : IFilter<IEnumerable<AnalysisSupplier>>
    {
        private readonly SupplierDeliveryDateTermsOptions _options;
        private readonly AnalysisCustomer _customer;

        public SupplierDeliveryDateTermsFilter(SupplierDeliveryDateTermsOptions options, AnalysisCustomer customer)
        {
            _options = options;
            _customer = customer;
        }

        public IEnumerable<AnalysisSupplier> Execute(IEnumerable<AnalysisSupplier> input)
        {
            if (_options == null || _options.DeliveryDateTerms == DeliveryDateTerms.Any) return input;

            if (_options.DeliveryDateTerms == DeliveryDateTerms.Min)
            {
                var suppliers = input.ToList();
                var minDate = suppliers.Where(q => q.DeliveryDate.HasValue).Min(q => q.DeliveryDate.Value);
                input = suppliers.Where(q => q.DeliveryDate.HasValue && q.DeliveryDate == minDate);
            }
            else if (_options.DeliveryDateTerms == DeliveryDateTerms.LessThanInRequest)
            {
                input = input.Where(q => q.DeliveryDate.HasValue && q.DeliveryDate <= _customer.DeliveryDate);
            }

            return input;
        }
    }
}


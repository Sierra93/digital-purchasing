using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Interfaces.Analysis.VariantOptions;

namespace DigitalPurchasing.Analysis.Filters
{
    public class SupplierDeliveryTermsFilter : IFilter<IEnumerable<AnalysisSupplier>>
    {
        private readonly SupplierDeliveryTermsOptions _options;

        public SupplierDeliveryTermsFilter(SupplierDeliveryTermsOptions options)
            => _options = options;

        public IEnumerable<AnalysisSupplier> Execute(IEnumerable<AnalysisSupplier> input)
        {
            if (_options != null && _options.DeliveryTerms != DeliveryTerms.NoRequirements)
            {
                return input.Where(q => q.DeliveryTerms == _options.DeliveryTerms);
            }

            return input;
        }
    }
}

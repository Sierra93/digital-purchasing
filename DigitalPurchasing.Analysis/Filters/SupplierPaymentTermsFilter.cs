using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Interfaces.Analysis.VariantOptions;

namespace DigitalPurchasing.Analysis.Filters
{
    public class SupplierPaymentTermsFilter : IFilter<IEnumerable<AnalysisSupplier>>
    {
        private readonly SupplierPaymentTermsOptions _options;

        public SupplierPaymentTermsFilter(SupplierPaymentTermsOptions options) => _options = options;

        public IEnumerable<AnalysisSupplier> Execute(IEnumerable<AnalysisSupplier> input)
        {
            if (_options != null && _options.PaymentTerms != PaymentTerms.NoRequirements)
            {
                return input.Where(q => q.PaymentTerms == _options.PaymentTerms);
            }

            return input;
        }
    }
}

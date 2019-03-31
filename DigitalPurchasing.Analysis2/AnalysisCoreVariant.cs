using System;
using DigitalPurchasing.Analysis2.Filters;

namespace DigitalPurchasing.Analysis2
{
    public class AnalysisCoreVariant
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public SupplierPaymentTermsOptions PaymentTermsOptions { get; set; } = new SupplierPaymentTermsOptions();
        public SupplierDeliveryTermsOptions DeliveryTermsOptions { get; set; } = new SupplierDeliveryTermsOptions();
        public SupplierDeliveryDateTermsOptions DeliveryDateTermsOptions { get; set; } = new SupplierDeliveryDateTermsOptions();

        public VariantsSuppliersCountOptions SuppliersCountOptions { get; set; } = new VariantsSuppliersCountOptions();
        public VariantsTotalValueOptions TotalValueOptions { get; set; } = new VariantsTotalValueOptions();
    }
}

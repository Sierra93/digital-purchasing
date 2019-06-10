using System;
using DigitalPurchasing.Core.Interfaces.Analysis.VariantOptions;

namespace DigitalPurchasing.Core.Interfaces.Analysis
{
    public class AnalysisVariantData
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

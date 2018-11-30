using DigitalPurchasing.Analysis2.Enums;
using DigitalPurchasing.Core;

namespace DigitalPurchasing.Analysis2
{
    public class AnalysisOptions
    {
        public DeliveryDateType? DeliveryDate { get; private set; }

        public void SetDeliveryDate(DeliveryDateType deliveryDate) => DeliveryDate = deliveryDate;

        public PaymentTerms? PaymentTerms { get; private set; }

        public void SetPaymentTerms(PaymentTerms paymentTerms) => PaymentTerms = paymentTerms;

        public DeliveryTerms? DeliveryTerms { get; private set; }

        public void SetDeliveryTerms(DeliveryTerms deliveryTerms) => DeliveryTerms = deliveryTerms;

        public decimal? TotalValue { get; private set; }

        public void SetTotalValue(decimal value) => TotalValue = value;

        public class SupplierCountOptions
        {
            public int Count { get; set; }
            public SupplierCountType Type { get; set; }
        }

        public SupplierCountOptions SupplierCount { get; private set; }

        public void SetSupplierCount(SupplierCountType type, int value)
        {
            if (SupplierCount == null) SupplierCount = new SupplierCountOptions();
            SupplierCount.Type = type;
            SupplierCount.Count = value;
        }
    }
}

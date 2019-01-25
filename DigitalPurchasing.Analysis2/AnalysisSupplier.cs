using System.Linq;
using DigitalPurchasing.Core;

namespace DigitalPurchasing.Analysis2
{
    public class AnalysisSupplier : AnalysisClient<SupplierItem>
    {
        public DeliveryTerms DeliveryTerms { get; set; }
        public PaymentTerms PaymentTerms { get; set; }

        public decimal TotalPrice => Items.Sum(q => q.TotalPrice);
    }
}

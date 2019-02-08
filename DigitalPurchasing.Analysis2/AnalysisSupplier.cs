using System.Linq;
using DigitalPurchasing.Core;

namespace DigitalPurchasing.Analysis2
{
    public class AnalysisSupplier : AnalysisClient<SupplierItem>, ICopyable<AnalysisSupplier>
    {
        public DeliveryTerms DeliveryTerms { get; set; }
        public PaymentTerms PaymentTerms { get; set; }

        public decimal TotalPrice => Items.Sum(q => q.TotalPrice);

        public AnalysisSupplier Copy() => new AnalysisSupplier
        {
            Id = Id,
            Date = Date,
            DeliveryTerms = DeliveryTerms,
            PaymentTerms = PaymentTerms,
            Items = Items.ConvertAll(q => q.Copy())
        };
    }
}

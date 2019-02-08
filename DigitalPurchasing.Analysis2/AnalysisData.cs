using System.Diagnostics;

namespace DigitalPurchasing.Analysis2
{
    [DebuggerDisplay("{Item.Id} | S:{Score}")]
    public class AnalysisData : ICopyable<AnalysisData>
    {
        public AnalysisSupplier Supplier { get; set; }
        public SupplierItem Item { get; set; }
        public decimal Score { get; set; }

        public AnalysisData Copy() => new AnalysisData
        {
            Score = Score,
            Item = Item.Copy(),
            Supplier = Supplier.Copy()
        };
    }
}

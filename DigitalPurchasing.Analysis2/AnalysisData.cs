using System.Diagnostics;

namespace DigitalPurchasing.Analysis2
{
    [DebuggerDisplay("{Item.Id} | S:{Score}")]
    public class AnalysisData : ICopyable<AnalysisData>
    {
        public AnalysisSupplier Supplier { get; set; }
        public SupplierItem Item { get; set; }

        public AnalysisData Copy() => new AnalysisData
        {
            Item = Item.Copy(),
            Supplier = Supplier.Copy()
        };
    }
}

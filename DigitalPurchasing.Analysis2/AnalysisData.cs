using System.Diagnostics;

namespace DigitalPurchasing.Analysis2
{
    [DebuggerDisplay("{Item.Id} | S:{Score}")]
    public class AnalysisData
    {
        public Supplier Supplier { get; set; }
        public SupplierItem Item { get; set; }
        public decimal Score { get; set; }
    }
}
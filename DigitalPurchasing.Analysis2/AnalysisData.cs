using System;
using System.Diagnostics;

namespace DigitalPurchasing.Analysis2
{
    [DebuggerDisplay("{Item.Id} | S:{Score}")]
    public class AnalysisData : ICopyable<AnalysisData>
    {
        public Guid SupplierId { get; set; }
        public SupplierItem Item { get; set; }

        public decimal CustomerQuantity { get; set; }

        public AnalysisData Copy() => new AnalysisData
        {
            Item = Item.Copy(),
            SupplierId = SupplierId
        };
    }
}

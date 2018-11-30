using System.Diagnostics;

namespace DigitalPurchasing.Analysis2
{
    [DebuggerDisplay("Id: {Id} | Q: {Quantity} | P: {Price}")]
    public class SupplierItem : CustomerItem
    {
        public decimal Price { get; set; }

        public decimal TotalPrice => Quantity * Price;
    }
}
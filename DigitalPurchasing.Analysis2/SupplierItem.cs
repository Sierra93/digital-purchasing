using System;
using System.Diagnostics;

namespace DigitalPurchasing.Analysis2
{
    [DebuggerDisplay("Id: {Id} | Q: {Quantity} | P: {Price}")]
    public class SupplierItem : ICopyable<SupplierItem>
    {
        public decimal Price { get; set; }

        public decimal TotalPrice => Quantity * Price;

        public Guid InternalId { get; set; } = Guid.NewGuid();
        public Guid Id { get; set; }
        public decimal Quantity { get; set; }

        public SupplierItem Copy() => new SupplierItem
        {
            Id = Id, InternalId = InternalId, Quantity = Quantity, Price = Price
        };
    }
}

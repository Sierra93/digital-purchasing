using System;
using System.Diagnostics;

namespace DigitalPurchasing.Analysis2
{
    [DebuggerDisplay("Id: {Id} | Q: {Quantity}")]
    public class CustomerItem : ICopyable<CustomerItem>
    {
        public Guid InternalId { get; set; } = Guid.NewGuid();
        public Guid Id { get; set; }
        public decimal Quantity { get; set; }

        public CustomerItem Copy() => new CustomerItem
        {
            Id = Id, InternalId = InternalId, Quantity = Quantity
        };
    }
}

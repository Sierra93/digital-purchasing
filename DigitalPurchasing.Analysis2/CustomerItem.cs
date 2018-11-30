using System;
using System.Diagnostics;

namespace DigitalPurchasing.Analysis2
{
    [DebuggerDisplay("Id: {Id} | Q: {Quantity}")]
    public class CustomerItem
    {
        public Guid InternalId { get; set; } = Guid.NewGuid();
        public Guid Id { get; set; }
        public int Quantity { get; set; }
    }
}
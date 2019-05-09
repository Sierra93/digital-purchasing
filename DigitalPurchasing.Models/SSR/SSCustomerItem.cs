using System;

namespace DigitalPurchasing.Models.SSR
{
    public class SSCustomerItem : SSBase
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }

        public SSCustomer Customer { get; set; }
        public Guid CustomerId { get; set; }

        public Guid InternalId { get; set; }
        public int Position { get; set; }

        public Guid NomenclatureId { get; set; }
    }
}

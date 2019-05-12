using System;

namespace DigitalPurchasing.Models.SSR
{
    public class SSSupplier : SSBase
    {
        public string Name { get; set; }
        public Guid InternalId { get; set; }

        public DateTime SOCreatedOn { get; set; }
        public int SONumber { get; set; }
    }
}

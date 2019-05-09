using System;

namespace DigitalPurchasing.Models.SSR
{
    public class SSCustomer : SSBase
    {
        public string Name { get; set; }
        public Guid InternalId { get; set; }
    }
}

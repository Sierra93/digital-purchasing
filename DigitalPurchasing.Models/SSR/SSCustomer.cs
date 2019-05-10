using System;

namespace DigitalPurchasing.Models.SSR
{
    public class SSCustomer : SSBase
    {
        public string Name { get; set; }
        public Guid InternalId { get; set; }

        public Guid ReportId { get; set; }
        public SSReport Report { get; set; }
    }
}

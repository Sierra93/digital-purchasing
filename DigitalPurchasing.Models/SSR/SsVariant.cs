using System;

namespace DigitalPurchasing.Models.SSR
{
    public class SSVariant : SSBase
    {
        public SSReport Report { get; set; }
        public Guid ReportId { get; set; }

        public int Number { get; set; }

        public bool IsSelected { get; set; }

        public Guid InternalId { get; set; }
    }
}

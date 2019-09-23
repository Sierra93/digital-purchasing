using System;

namespace DigitalPurchasing.Models.SSR
{
    public class SSSupplier : SSBase
    {
        public string Name { get; set; }

        // Supplier Id
        public Guid InternalId { get; set; }

        public DateTime SOCreatedOn { get; set; }
        public int SONumber { get; set; }

        // SupplierOffer Id
        public Guid SOInternalId { get; set; }

        public SSReport Report { get; set;}
        public Guid ReportId { get; set; }
    }
}

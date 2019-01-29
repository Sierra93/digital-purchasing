using System;

namespace DigitalPurchasing.Models
{
    public class NomenclatureAlternativeLink
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AlternativeId { get; set; }
        public NomenclatureAlternative Alternative { get; set; }

        public Guid? CustomerId { get; set; }
        public Customer Customer { get; set; }

        public Guid? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}

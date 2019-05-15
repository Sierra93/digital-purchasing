using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalPurchasing.Models.SSR
{
    public class SSCustomerItem : SSBase
    {
        public string Name { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Quantity { get; set; }

        public string Code { get; set; }

        public string Uom { get; set; }

        public SSCustomer Customer { get; set; }
        public Guid CustomerId { get; set; }

        public Guid InternalId { get; set; }
        public int Position { get; set; }

        public Guid NomenclatureId { get; set; }
    }
}

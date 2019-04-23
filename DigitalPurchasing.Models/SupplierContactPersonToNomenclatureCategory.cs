using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public class SupplierContactPersonToNomenclatureCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public NomenclatureCategory NomenclatureCategory { get; set; }
        public Guid NomenclatureCategoryId { get; set; }

        public SupplierContactPerson SupplierContactPerson { get; set; }
        public Guid SupplierContactPersonId { get; set; }

        public bool IsPrimaryContact { get; set; }
    }
}

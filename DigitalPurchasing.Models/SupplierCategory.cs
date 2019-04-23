using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Models
{
    public class SupplierCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public NomenclatureCategory NomenclatureCategory { get; set; }
        public Guid NomenclatureCategoryId { get; set; }

        public SupplierContactPerson PrimaryContactPerson { get; set; }
        public Guid? PrimaryContactPersonId { get; set; }

        public SupplierContactPerson SecondaryContactPerson { get; set; }
        public Guid? SecondaryContactPersonId { get; set; }
    }
}

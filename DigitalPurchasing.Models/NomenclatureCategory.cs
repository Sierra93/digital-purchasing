using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Models
{
    public class NomenclatureCategory : BaseModelWithOwner
    {
        public string Name { get; set; }

        public Guid? ParentId { get; set; }
        public NomenclatureCategory Parent { get; set; }

        public ICollection<NomenclatureCategory> Children { get; set; }

        public ICollection<Nomenclature> Nomenclatures { get; set; }
    }
}
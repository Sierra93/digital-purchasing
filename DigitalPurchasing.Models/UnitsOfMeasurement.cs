using System;
using System.Collections.Generic;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Models
{
    public class UnitsOfMeasurement : BaseModel, IMayHaveOwner
    {
        public string Name { get; set; }

        public Guid? OwnerId { get; set; } // null = common/system
        public Company Owner { get; set; }

        public ICollection<Nomenclature> BatchNomenclatures { get; set; }
        public ICollection<Nomenclature> MassNomenclatures { get; set; }
        public ICollection<Nomenclature> ResourceNomenclatures { get; set; }
        public ICollection<Nomenclature> ResourceBatchNomenclatures { get; set; }
    }
}

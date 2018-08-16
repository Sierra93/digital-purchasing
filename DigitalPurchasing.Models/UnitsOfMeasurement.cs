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

        public ICollection<Nomenclature> BasicNomenclatures { get; set; }
        public ICollection<Nomenclature> MassNomenclatures { get; set; }
        public ICollection<Nomenclature> CycleNomenclatures { get; set; }
    }
}

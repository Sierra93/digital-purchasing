using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Models
{
    public class UnitsOfMeasurement : BaseModel
    {
        public string Name { get; set; }

        public Guid? OwnerId { get; set; } // null = common/system
        public Company Owner { get; set; }

        public ICollection<Nomenclature> BasicNomenclatures { get; set; }
        public ICollection<Nomenclature> MassNomenclatures { get; set; }
        public ICollection<Nomenclature> CycleNomenclatures { get; set; }
    }

    public class Nomenclature : BaseModelWithOwner
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public Guid BasicUoMId { get; set; }
        public UnitsOfMeasurement BasicUoM { get; set; }

        public Guid MassUoMId { get; set; }
        public UnitsOfMeasurement MassUoM { get; set; }

        public Guid CycleUoMId { get; set; }
        public UnitsOfMeasurement CycleUoM { get; set; }

        public Guid CategoryId { get; set; }
        public NomenclatureCategory Category { get; set; }
    }
}

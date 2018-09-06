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

        public ICollection<UomConversionRate> FromConversionRates { get; set; }
        public ICollection<UomConversionRate> ToConversionRates { get; set; }
    }

    public class UomConversionRate : BaseModel, IMayHaveOwner
    {
        public Guid? OwnerId { get; set; } // null = common/system
        public Company Owner { get; set; }

        // 1 From UoM = {factor} To UoM
        public Guid FromUomId { get; set; }
        public UnitsOfMeasurement FromUom { get; set; }

        public decimal Factor { get; set; }

        public Guid ToUomId { get; set; }
        public UnitsOfMeasurement ToUom { get; set; }

        public Guid? NomenclatureId { get; set; }
        public Nomenclature Nomenclature { get; set; }
    }
}

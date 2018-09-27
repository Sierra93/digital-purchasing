using System;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Models
{
    public class UomConversionRate : BaseModel, IMayHaveOwner
    {
        public Guid? OwnerId { get; set; } // null = common/system
        public Company Owner { get; set; }

        // 1 From UoM = {factor} To UoM
        public Guid FromUomId { get; set; }
        public UnitsOfMeasurement FromUom { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Factor { get; set; }

        public Guid ToUomId { get; set; }
        public UnitsOfMeasurement ToUom { get; set; }

        public Guid? NomenclatureId { get; set; }
        public Nomenclature Nomenclature { get; set; }
    }
}

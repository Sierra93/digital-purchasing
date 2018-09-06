using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Models
{
    public class Nomenclature : BaseModelWithOwner
    {
        public string Code { get; set; }

        public string Name { get; set; }
        public string NameEng { get; set; }

        public Guid BatchUomId { get; set; }
        public UnitsOfMeasurement BatchUom { get; set; }

        public Guid MassUomId { get; set; }
        public UnitsOfMeasurement MassUom { get; set; }
        public decimal MassUomValue { get; set; }
       
        public Guid ResourceUomId { get; set; }
        public UnitsOfMeasurement ResourceUom { get; set; }
        public decimal ResourceUomValue { get; set; }

        public Guid ResourceBatchUomId { get; set; }
        public UnitsOfMeasurement ResourceBatchUom { get; set; }

        public Guid CategoryId { get; set; }
        public NomenclatureCategory Category { get; set; }

        public ICollection<UomConversionRate> ConversionRates { get; set; }
    }
}

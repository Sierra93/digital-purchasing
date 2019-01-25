using System;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalPurchasing.Core;

namespace DigitalPurchasing.Models
{
    public class NomenclatureAlternativeLink
    {
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }

    public class NomenclatureAlternative : BaseModelWithOwner
    {
        public Guid NomenclatureId { get; set; }
        public Nomenclature Nomenclature { get; set; }

        public ClientType ClientType { get; set; }
        public string ClientName { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

        public Guid? BatchUomId { get; set; }
        public UnitsOfMeasurement BatchUom { get; set; }

        public Guid? MassUomId { get; set; }
        public UnitsOfMeasurement MassUom { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal MassUomValue { get; set; }
       
        public Guid? ResourceUomId { get; set; }
        public UnitsOfMeasurement ResourceUom { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal ResourceUomValue { get; set; }

        public Guid? ResourceBatchUomId { get; set; }
        public UnitsOfMeasurement ResourceBatchUom { get; set; }
    }
}

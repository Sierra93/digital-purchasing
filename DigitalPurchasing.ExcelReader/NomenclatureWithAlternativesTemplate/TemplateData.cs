using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.ExcelReader.NomenclatureWithAlternativesTemplate
{
    public sealed class TemplateData
    {
        public string AlternativesRowType { get; set; }
        public string InternalCode { get; set; }
        public string ClientName { get; set; }
        public string CategoryName { get; set; }
        public string NomenclatureCode { get; set; }
        public string NomenclatureName { get; set; }
        public string NomenclatureEngName { get; set; }
        public string AlternativeCode { get; set; }
        public string AlternativeName { get; set; }
        public string BatchUomName { get; set; }
        public string MassUomName { get; set; }
        public decimal? MassUomValue { get; set; }

        // TODO
        public string Website { get; set; }

        // TODO
        public string SupplierPhone { get; set; }

        public string ResourceUomName { get; set; }
        public decimal? ResourceUomValue { get; set; }
        public string ResourceBatchUomName { get; set; }
    }
}

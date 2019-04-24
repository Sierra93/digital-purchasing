using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using EPPlus.Core.Extensions;
using OfficeOpenXml;

namespace DigitalPurchasing.ExcelReader
{
    public class ExcelTemplate
    {
        [ExcelWorksheet("Template")]
        private class TemplateDataInternal
        {
            [ExcelTableColumn(1)]
            public string Category { get; set; }
            [ExcelTableColumn(2)]
            public string Code { get; set; }
            [ExcelTableColumn(3)]
            public string Name { get; set; }
            [ExcelTableColumn(4)]
            public string NameEng { get; set; }
            [ExcelTableColumn(5)]
            public string Uom { get; set; }

            [ExcelTableColumn(6)]
            public string UomMass { get; set; }
            [ExcelTableColumn(7)]
            public string UomMassValue { get; set; }

            [ExcelTableColumn(8)]
            public string ResourceUom { get; set; }
            [ExcelTableColumn(9)]
            public string ResourceUomValue { get; set; }

            [ExcelTableColumn(10)]
            public string ResourceBatchUom { get; set; }
        }

        public byte[] Build()
        {
            using (var excel = new ExcelPackage())
            {
                var ws = excel.Workbook.Worksheets.Add("Template");
                ws.Cells[1, 1].Value = "Категория";
                ws.Cells[1, 2].Value = "Код";
                ws.Cells[1, 3].Value = "Название";
                ws.Cells[1, 4].Value = "Название Eng";
                ws.Cells[1, 5].Value = "EИ";
                ws.Cells[1, 6].Value = "EИ массы";
                ws.Cells[1, 7].Value = "Масса 1 ЕИ, ЕИ массы";
                ws.Cells[1, 8].Value = "Название ресурса";
                ws.Cells[1, 9].Value = "Ресурс, 1 ЕИ ресурса";
                ws.Cells[1, 10].Value = "ЕИ ресурса";
                ws.Column(1).AutoFit();
                ws.Column(2).AutoFit();
                ws.Column(3).AutoFit();
                ws.Column(4).AutoFit();
                ws.Column(5).AutoFit();
                ws.Column(6).AutoFit();
                ws.Column(7).AutoFit();
                ws.Column(8).AutoFit();
                ws.Column(9).AutoFit();
                ws.Column(10).AutoFit();
                return excel.GetAsByteArray();
            }
        }

        public List<TemplateData> Read(string filePath)
        {
            using (var excel = new ExcelPackage(new FileInfo(filePath)))
            {
                var items = excel.ToList<TemplateDataInternal>(configurationAction: c =>
                {
                    c.WithoutHeaderRow();
                    c.SkipCastingErrors();
                }).Where(q => q.Code != "Код"); // c.WithoutHeaderRow(); don't work for some reason, bug?

                var result = items.Select(q => new TemplateData
                {
                    Category = q.Category,
                    Code = q.Code,
                    Name = q.Name,
                    NameEng = q.NameEng,
                    Uom = q.Uom,
                    UomMass = q.UomMass,
                    ResourceUom = q.ResourceUom,
                    ResourceBatchUom = q.ResourceBatchUom,
                    UomMassValue = ToNullableDecimal(q.UomMassValue?.Replace(".", ",")) ?? 0,
                    ResourceUomValue = ToNullableDecimal(q.ResourceUomValue?.Replace(".", ",")) ?? 0
                }).ToList(); 

                return result;
            }
        }

        private static decimal? ToNullableDecimal(string s) => decimal.TryParse(s, out var i) ? (decimal?)i : null;
    }

    public class TemplateData
    {
        public string Category { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameEng { get; set; }
        public string Uom { get; set; }
        public string UomMass { get; set; }
        public decimal UomMassValue { get; set; }
        public string ResourceUom { get; set; }
        public decimal ResourceUomValue { get; set; }
        public string ResourceBatchUom { get; set; }
    }
}

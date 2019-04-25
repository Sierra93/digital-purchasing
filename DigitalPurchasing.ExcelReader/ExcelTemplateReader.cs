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
            public string PackUomValue { get; set; }

            [ExcelTableColumn(9)]
            public string PackUom { get; set; }

            [ExcelTableColumn(10)]
            public string ResourceUom { get; set; }
            [ExcelTableColumn(11)]
            public string ResourceUomValue { get; set; }

            [ExcelTableColumn(12)]
            public string ResourceBatchUom { get; set; }
        }

        public byte[] Build()
        {
            using (var excel = new ExcelPackage())
            {
                var ws = excel.Workbook.Worksheets.Add("Template");
                var cols = new string[]
                {
                    "Категория",
                    "Код",
                    "Название",
                    "Название Eng",
                    "EИ",
                    "EИ массы",
                    "Масса 1 ЕИ, ЕИ массы",
                    "Количество товара в упаковке, ЕИ товара в упаковке",
                    "ЕИ товара в упаковке",
                    "Название ресурса",
                    "Ресурс, 1 ЕИ ресурса",
                    "ЕИ ресурса",
                };
                for (var i = 0; i < cols.Length; i++)
                {
                    ws.Cells[1, i + 1].Value = cols[i];
                    ws.Column(i + 1).AutoFit();
                }
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
                    UomMassValue = ToNullableDecimal(q.UomMassValue) ?? 0,
                    ResourceUomValue = ToNullableDecimal(q.ResourceUomValue) ?? 0,
                    PackUomValue = ToNullableDecimal(q.PackUomValue),
                    PackUom = q.PackUom
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
        public decimal? PackUomValue { get; set; }
        public string PackUom { get; set; }
    }
}

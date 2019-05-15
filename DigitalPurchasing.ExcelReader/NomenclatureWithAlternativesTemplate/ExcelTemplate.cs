using EPPlus.Core.Extensions;
using Mapster;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DigitalPurchasing.ExcelReader.NomenclatureWithAlternativesTemplate
{
    public class ExcelTemplate
    {
        private const string WORKSHEET_NAME = "Справочник аналогов";

        [ExcelWorksheet(WORKSHEET_NAME)]
        private class TemplateDataInternal
        {
            [ExcelTableColumn(1)]
            public string AlternativesRowType { get; set; }

            [ExcelTableColumn(2)]
            public string ClientPublicId { get; set; }

            [ExcelTableColumn(3)]
            public string ClientName { get; set; }

            [ExcelTableColumn(4)]
            public string CategoryName { get; set; }

            [ExcelTableColumn(5)]
            public string NomenclatureCode { get; set; }

            [ExcelTableColumn(6)]
            public string NomenclatureName { get; set; }

            [ExcelTableColumn(7)]
            public string NomenclatureEngName { get; set; }

            [ExcelTableColumn(8)]
            public string AlternativeCode { get; set; }

            [ExcelTableColumn(9)]
            public string AlternativeName { get; set; }

            [ExcelTableColumn(10)]
            public string BatchUomName { get; set; }

            [ExcelTableColumn(11)]
            public string MassUomName { get; set; }

            [ExcelTableColumn(12)]
            public string MassUomValue { get; set; }

            [ExcelTableColumn(13)]
            public string PackUomValue { get; set; }

            [ExcelTableColumn(14)]
            public string PackUomName { get; set; }

            [ExcelTableColumn(15)]
            public string ResourceUomName { get; set; }

            [ExcelTableColumn(16)]
            public string ResourceUomValue { get; set; }

            [ExcelTableColumn(17)]
            public string ResourceBatchUomName { get; set; }
        }

        public byte[] Build(object[][] data)
        {
            using (var excel = new ExcelPackage())
            {
                var ws = excel.Workbook.Worksheets.Add(WORKSHEET_NAME);
                var cols = new string[]
                {
                    "Тип",
                    "Внутренний код организации\nили клиента",
                    "Название организации или\nвнутреннего клиента",
                    "Категория",
                    "Код базовой\nноменклатуры",
                    "Название базовой\nноменклатуры",
                    "Название Eng базовой\nноменклатуры",
                    "Код аналога",
                    "Название аналога\nв справочнике организации",
                    "ЕИ",
                    "EИ массы",
                    "Масса 1 ЕИ,\nЕИ массы",
                    "Количество товара в упаковке,\nЕИ товара в упаковке",
                    "ЕИ товара в\nупаковке",
                    "Название\nресурса",
                    "Ресурс аналога,\n1 ЕИ ресурса",
                    "ЕИ ресурса"
                };
                for (var i = 0; i < cols.Length; i++)
                {
                    ws.Cells[1, i + 1].Value = cols[i];
                    ws.Cells[1, i + 1].Style.Font.Bold = true;
                    ws.Cells[1, i + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                }

                for (var i = 0; i < data.Length; i++)
                {
                    for (var j = 0; j < data[i].Length; j++)
                    {
                        ws.Cells[i + 2, j + 1].Value = data[i][j];
                    }
                }

                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                ws.View.FreezePanes(2, 1);

                return excel.GetAsByteArray();
            }
        }

        public IEnumerable<TemplateData> Read(string filePath)
        {
            using (var excel = new ExcelPackage(new FileInfo(filePath)))
            {
                var items = excel.ToList<TemplateDataInternal>();

                var config = new TypeAdapterConfig();
                config.ForType<TemplateDataInternal, TemplateData>()
                    .Map(dest => dest.MassUomValue, src => ToNullableDecimal(src.MassUomValue))
                    .Map(dest => dest.ClientPublicId, src => ToNullableInt(src.ClientPublicId))
                    .Map(dest => dest.PackUomValue, src => ToNullableInt(src.PackUomValue))
                    .Map(dest => dest.ResourceUomValue, src => ToNullableDecimal(src.ResourceUomValue));
                var result = items.Select(q => q.Adapt<TemplateData>(config)).ToList();

                return result;
            }
        }

        private static decimal? ToNullableDecimal(string s) => decimal.TryParse(s, out var i) ? (decimal?)i : null;
        private static int? ToNullableInt(string s) => int.TryParse(s, out var i) ? (int?)i : null;
    }
}

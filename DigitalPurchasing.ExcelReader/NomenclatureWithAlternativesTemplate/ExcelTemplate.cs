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
            public string MainCategory { get; set; }

            [ExcelTableColumn(3)]
            public string SubCategory1 { get; set; }

            [ExcelTableColumn(4)]
            public string SubCategory2 { get; set; }

            [ExcelTableColumn(5)]
            public string OwnershipType { get; set; }

            [ExcelTableColumn(6)]
            public string Inn { get; set; }

            [ExcelTableColumn(7)]
            public string ErpCode { get; set; }

            [ExcelTableColumn(8)]
            public string SupplierType { get; set; }

            [ExcelTableColumn(9)]
            public string PaymentDeferredDays { get; set; }

            [ExcelTableColumn(10)]
            public string DeliveryTerms { get; set; }

            [ExcelTableColumn(11)]
            public string OfferCurrency { get; set; }

            [ExcelTableColumn(12)]
            public string PriceWithVat { get; set; }

            [ExcelTableColumn(13)]
            public string Website { get; set; }

            [ExcelTableColumn(14)]
            public string SupplierPhone { get; set; }

            [ExcelTableColumn(15)]
            public string ContactFirstName { get; set; }

            [ExcelTableColumn(16)]
            public string ContactLastName { get; set; }

            [ExcelTableColumn(17)]
            public string ContactJobTitle { get; set; }

            [ExcelTableColumn(18)]
            public string ContactEmail { get; set; }

            [ExcelTableColumn(19)]
            public string ContactPhone { get; set; }

            [ExcelTableColumn(20)]
            public string ContactMobilePhone { get; set; }

            [ExcelTableColumn(21)]
            public string Note { get; set; }

            [ExcelTableColumn(22)]
            public string LegalAddressStreet { get; set; }

            [ExcelTableColumn(23)]
            public string LegalAddressCity { get; set; }

            [ExcelTableColumn(24)]
            public string LegalAddressCountry { get; set; }

            [ExcelTableColumn(25)]
            public string ActualAddressStreet { get; set; }

            [ExcelTableColumn(26)]
            public string ActualAddressCity { get; set; }

            [ExcelTableColumn(27)]
            public string ActualAddressCountry { get; set; }

            [ExcelTableColumn(28)]
            public string WarehouseAddressStreet { get; set; }

            [ExcelTableColumn(29)]
            public string WarehouseAddressCity { get; set; }

            [ExcelTableColumn(30)]
            public string WarehouseAddressCountry { get; set; }
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

                return excel.GetAsByteArray();
            }
        }

        public List<TemplateData> Read(string filePath)
        {
            using (var excel = new ExcelPackage(new FileInfo(filePath)))
            {
                var items = excel.ToList<TemplateDataInternal>();

                //var config = new TypeAdapterConfig();
                //config.ForType<TemplateDataInternal, TemplateData>()
                //    .Map(dest => dest.PriceWithVat, src => src.PriceWithVat != null && src.PriceWithVat.Equals("да", StringComparison.InvariantCultureIgnoreCase))
                //    .Map(dest => dest.Inn, src => ToNullableLong(src.Inn))
                //    .Map(dest => dest.PaymentDeferredDays, src => ToNullableInt(src.PaymentDeferredDays));
                var result = items.Select(q => q.Adapt<TemplateData>()).ToList();

                return result;
            }
        }

        private static decimal? ToNullableDecimal(string s) => decimal.TryParse(s, out var i) ? (decimal?)i : null;
        private static long? ToNullableLong(string s) => long.TryParse(s, out var i) ? (long?)i : null;
        private static int? ToNullableInt(string s) => int.TryParse(s, out var i) ? (int?)i : null;
    }
}

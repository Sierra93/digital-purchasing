using EPPlus.Core.Extensions;
using Mapster;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DigitalPurchasing.ExcelReader.NomenclatureCategoryListTemplate
{
    public class ExcelTemplate
    {
        private const string WORKSHEET_NAME = "Категории закупок";

        [ExcelWorksheet(WORKSHEET_NAME)]
        private class TemplateDataInternal
        {
            [ExcelTableColumn(1)]
            public string MainCategory { get; set; }

            [ExcelTableColumn(2)]
            public string SubCategory1 { get; set; }

            [ExcelTableColumn(3)]
            public string SubCategory2 { get; set; }
        }

        public byte[] Build()
        {
            using (var excel = new ExcelPackage())
            {
                var ws = excel.Workbook.Worksheets.Add(WORKSHEET_NAME);
                var cols = new string[]
                {
                    "Основная категория",
                    "Подкатегория 1 основной категории",
                    "Подкатегория 2 основной категории"
                };
                for (var i = 0; i < cols.Length; i++)
                {
                    ws.Cells[1, i + 1].Value = cols[i];
                    ws.Cells[1, i + 1].Style.Font.Bold = true;
                    ws.Column(i + 1).AutoFit();
                }
                return excel.GetAsByteArray();
            }
        }

        //public List<TemplateData> Read(string filePath)
        //{
        //    using (var excel = new ExcelPackage(new FileInfo(filePath)))
        //    {
        //        var items = excel.ToList<TemplateDataInternal>();

        //        var config = new TypeAdapterConfig();
        //        config.ForType<TemplateDataInternal, TemplateData>()
        //            .Map(dest => dest.PriceWithVat, src => src.PriceWithVat != null && src.PriceWithVat.Equals("да", StringComparison.InvariantCultureIgnoreCase))
        //            .Map(dest => dest.Inn, src => ToNullableLong(src.Inn))
        //            .Map(dest => dest.PaymentDeferredDays, src => ToNullableInt(src.PaymentDeferredDays));
        //        var result = items.Select(q => q.Adapt<TemplateData>(config)).ToList();

        //        return result;
        //    }
        //}

        //private static decimal? ToNullableDecimal(string s) => decimal.TryParse(s, out var i) ? (decimal?)i : null;
        //private static long? ToNullableLong(string s) => long.TryParse(s, out var i) ? (long?)i : null;
        //private static int? ToNullableInt(string s) => int.TryParse(s, out var i) ? (int?)i : null;
    }
}

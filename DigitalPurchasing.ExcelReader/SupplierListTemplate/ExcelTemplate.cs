using EPPlus.Core.Extensions;
using Mapster;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DigitalPurchasing.ExcelReader.SupplierListTemplate
{
    public class ExcelTemplate
    {
        private const string WORKSHEET_NAME = "Справочник поставщиков";

        [ExcelWorksheet(WORKSHEET_NAME)]
        private class TemplateDataInternal
        {
            [ExcelTableColumn(1)]
            public string SupplierName { get; set; }

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

        public byte[] Build()
        {
            using (var excel = new ExcelPackage())
            {
                var ws = excel.Workbook.Worksheets.Add(WORKSHEET_NAME);
                var cols = new string[]
                {
                    "Название поставщика",
                    "Основная категория 1",
                    "Подкатегория 1 основной категории 1",
                    "Подкатегория 2 основной категории 1",
                    "Организационная форма юр лица",
                    "ИНН",
                    "Код поставщика в ERP",
                    "Тип поставщика",
                    "Отсрочка платежа, дней",
                    "Условия поставки (incoterms)",
                    "Валюта выставления предложений",
                    "Цены в предложении указаны с НДС?",
                    "Сайт",
                    "Телефон общий",
                    "Контакт №1 (ключевой): имя",
                    "Контакт №1 (ключевой): Фамилия",
                    "Должность контакта 1",
                    "Контакт №1 Почта",
                    "Контакт №1 Телефон",
                    "Контакт №1 Телефон моб",
                    "Комментарии",
                    "Юридический адрес: улица",
                    "Юридический адрес: город",
                    "Юридический адрес: страна",
                    "Фактический адрес: улица",
                    "Фактический адрес: город",
                    "Фактический адрес: страна",
                    "Адрес склада №1: улица",
                    "Адрес склада №1: город",
                    "Адрес склада №1: страна"
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

        public List<TemplateData> Read(string filePath)
        {
            using (var excel = new ExcelPackage(new FileInfo(filePath)))
            {
                var items = excel.ToList<TemplateDataInternal>();

                var config = new TypeAdapterConfig();
                config.ForType<TemplateDataInternal, TemplateData>()
                    .Map(dest => dest.PriceWithVat, src => src.PriceWithVat != null && src.PriceWithVat.Equals("да", StringComparison.InvariantCultureIgnoreCase))
                    .Map(dest => dest.Inn, src => ToNullableLong(src.Inn));
                var result = items.Select(q => q.Adapt<TemplateData>(config)).ToList();

                return result;
            }
        }

        private static decimal? ToNullableDecimal(string s) => decimal.TryParse(s, out var i) ? (decimal?)i : null;
        private static long? ToNullableLong(string s) => long.TryParse(s, out var i) ? (long?)i : null;
    }
}

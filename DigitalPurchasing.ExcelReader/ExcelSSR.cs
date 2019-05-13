using System;
using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace DigitalPurchasing.ExcelReader
{
    public class ExcelSSR
    {
        private readonly SSReportDto _report;

        public ExcelSSR(SSReportDto report) => _report = report;

        public byte[] Build()
        {
            using (var excel = new ExcelPackage())
            {
                var ws = excel.Workbook.Worksheets.Add("Подробности");

                var suppliersCount = _report.Suppliers.Count;

                ws.Column(1).Width = 1.33;
                ws.Column(2).AutoFit();
                ws.Column(3).Width = 14.67;
                ws.Column(4).Width = 27.67;
                ws.Column(5).Width = 10;
                ws.Column(6).Width = 10;
                ws.Column(7).Width = 0.73; // separator 1
                //ws.Column(8).AutoFit(19);
                ws.Column(7 + suppliersCount + 1).Width = 0.73; // separator 2
                ws.Column(7 + suppliersCount + 1 + suppliersCount + 1).Width = 0.73; // separator 3

                ws.Cells[1, 2].HeaderText("Анализ коммерческих предложений, приведенных к единицам измерения запроса")
                    .AlignLeft().NoWrapText();
                ws.Cells[2, 2].Value = $"Конкурентный лист № {_report.CLNumber} от {_report.CLCreatedOn:dd.MM.yyyy HH:mm}";

                ws.Cells[4, 2].Value = $"Клиент: {_report.Customer.Name}";
                ws.Cells[4, 4].Value = $"Заявка № {_report.Customer.PRNumber} от {_report.Customer.PRCreatedOn:dd.MM.yyyy HH:mm}";

                var colQuantityStart = 8;
                ws.Cells[4, colQuantityStart].HeaderText("Кол-во КП в ЕИ запроса").AlignLeft();//.NoWrapText();
                ws.Cells[4, colQuantityStart, 4, colQuantityStart + suppliersCount].Merge = true;
                foreach (var supplier in _report.Suppliers.OrderBy(q => q.SOCreatedOn))
                {
                    var pos = _report.Suppliers.IndexOf(supplier);
                    var col = colQuantityStart + pos;
                    ws.Cells[5, colQuantityStart + pos].HeaderText(supplier.Name);
                    foreach (var item in _report.SSSupplierItems.Where(q => q.SupplierId == supplier.Id))
                    {
                        var nomPos = GetPositionByNomenclature(item.NomenclatureId);
                        ws.Cells[6 + nomPos, col].TableText(item.Quantity);
                    }
                }

                var colPriceStart = colQuantityStart + suppliersCount + 1;
                ws.Cells[4, colPriceStart].HeaderText("Цены в валюте запроса за ЕИ запроса").AlignLeft();//.NoWrapText();
                ws.Cells[4, colPriceStart, 4, colPriceStart + suppliersCount].Merge = true;
                foreach (var supplier in _report.Suppliers.OrderBy(q => q.SOCreatedOn))
                {
                    var pos = _report.Suppliers.IndexOf(supplier);
                    var col = colPriceStart + pos;
                    ws.Cells[5, col].HeaderText(supplier.Name);
                    foreach (var item in _report.SSSupplierItems.Where(q => q.SupplierId == supplier.Id))
                    {
                        var nomPos = GetPositionByNomenclature(item.NomenclatureId);
                        ws.Cells[6 + nomPos, col].TableText(item.Price);
                    }
                }

                var colTotalPriceStart = colPriceStart + suppliersCount + 1;
                ws.Cells[4, colTotalPriceStart].HeaderText("Стоимость в валюте запроса").AlignLeft();//.NoWrapText();
                ws.Cells[4, colTotalPriceStart, 4, colTotalPriceStart + suppliersCount].Merge = true;
                foreach (var supplier in _report.Suppliers.OrderBy(q => q.SOCreatedOn))
                {
                    var pos = _report.Suppliers.IndexOf(supplier);
                    var col = colTotalPriceStart + pos;
                    ws.Column(col).AutoFit();
                    ws.Cells[5, colTotalPriceStart + pos].HeaderText(supplier.Name);
                    var supplierItems = _report.SSSupplierItems.Where(q => q.SupplierId == supplier.Id).ToList();
                    foreach (var item in supplierItems)
                    {
                        var nomPos = GetPositionByNomenclature(item.NomenclatureId);
                        ws.Cells[6 + nomPos, col].TableText(item.Price * item.Quantity);
                    }
                    ws.Cells[6 + supplierItems.Count, col].TableText(supplierItems.Sum(item => item.Price * item.Quantity)).BoldFont();
                }

                ws.Row(5).Height = 48;
                ws.Cells[5, 2].HeaderText("№");
                ws.Cells[5, 3].HeaderText("Код");
                ws.Cells[5, 4].HeaderText("Наименование");
                ws.Cells[5, 5].HeaderText("ЕИ");
                ws.Cells[5, 6].HeaderText("Кол-во для заказа, ЕИ");

                var i = 6;
                foreach (var data in _report.CustomerItems.OrderBy(q => q.Position))
                {
                    ws.Row(i).Height = 48;
                    ws.Cells[i, 2].TableText(data.Position);
                    ws.Cells[i, 3].TableText(data.Code);
                    ws.Cells[i, 4].TableText(data.Name).AlignLeft();
                    ws.Cells[i, 5].TableText(data.Uom); 
                    ws.Cells[i, 6].TableText(data.Quantity);
                    i++;
                }

                var titleRow = i + 4;
                var headerRow = titleRow + 2;
                var dataRow = headerRow + 1;

                ws.Row(i).Height = 48;
                ws.Cells[i, 4].HeaderText("ИТОГО");

                ws.Cells[titleRow, 4].Value = "Решение о выборе поставщиков";

                ws.Row(headerRow).Height = 48;
                ws.Cells[headerRow, 3].HeaderText("Дата и время подтверждения");
                ws.Cells[headerRow, 4].HeaderText("Вариант выбора");
                ws.Cells[headerRow, 5].HeaderText("Общая сумма");
                ws.Cells[headerRow, 6].HeaderText("Валюта");
                ws.Cells[headerRow, 8].HeaderText("Выбор подтвердил");

                ws.Row(dataRow).Height = 48;

                var orderedVariants = _report.Variants.OrderBy(q => q.CreatedOn).ToList();
                var selectedVariant = orderedVariants.Find(q => q.IsSelected);
                var selectedVariantIndex = orderedVariants.IndexOf(selectedVariant);
                var selectedVariantData = _report.Datas.Where(q => q.VariantId == selectedVariant.Id).ToList();

                ws.Cells[dataRow, 3].TableText(_report.CreatedOn.ToString("dd.MM.yyyy HH:mm"));
                ws.Cells[dataRow, 4].TableText((selectedVariantIndex + 1).ToString());
                ws.Cells[dataRow, 5].TableText(selectedVariantData.Sum(q => q.Quantity * GetSupplierPrice(q.SupplierId, q.NomenclatureId)));
                ws.Cells[dataRow, 6].TableText("RUB"); // todo
                ws.Cells[dataRow, 8].TableText($"{_report.User.LastName} {_report.User.FirstName}");

                return excel.GetAsByteArray();
            }
        }

        private int GetPositionByNomenclature(Guid nomenclatureId)
        {
            var customerItems = _report.CustomerItems.OrderBy(q => q.Position).ToList();
            return customerItems.IndexOf(customerItems.Find(q => q.NomenclatureId == nomenclatureId));
        }

        private decimal GetSupplierPrice(Guid supplierId, Guid nomenclatureId)
            => _report.SSSupplierItems.Find(q => q.SupplierId == supplierId && q.NomenclatureId == nomenclatureId).Price;
    }

    public static class Extensions
    {
        public static ExcelRange HeaderText(this ExcelRange cell, string text)
        {
            cell.Style.Font.Bold = true;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.WrapText = true;
            cell.Value = text;
            return cell;
        }

        public static ExcelRange TableText(this ExcelRange cell, int value)
        {
            if (value > 0)
            {
                cell.Style.Numberformat.Format = "### ### ##0.00";
            }
            cell.Value = value;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            return cell;
        }

        public static ExcelRange TableText(this ExcelRange cell, decimal value)
        {
            if (value > 0)
            {
                cell.Style.Numberformat.Format = "### ### ##0.00";
            }
            cell.Value = value;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            return cell;
        }

        public static ExcelRange TableText(this ExcelRange cell, string text)
        {
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Value = text;
            return cell;
        }

        public static ExcelRange AlignLeft(this ExcelRange cell)
        {
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            return cell;
        }
        
        public static ExcelRange AlignRight(this ExcelRange cell)
        {
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            return cell;
        }

        public static ExcelRange NoWrapText(this ExcelRange cell)
        {
            cell.Style.WrapText = false;
            return cell;
        }

        public static ExcelRange BoldFont(this ExcelRange cell)
        {
            cell.Style.Font.Bold = true;
            return cell;
        }
    }
}

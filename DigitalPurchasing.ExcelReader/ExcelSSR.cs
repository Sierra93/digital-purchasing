using System;
using System.Collections.Generic;
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

                var orderedCustomerItems = _report.CustomerItems.OrderBy(q => q.Position).ToList();

                var suppliersCount = _report.Suppliers.Count;

                var datasByVariants = _report.Datas
                    .GroupBy(q => q.Variant, new SSVariantDtoComparer())
                    .OrderBy(q => q.Key.Number)
                    .ToList();

                var itemsCountByVariants = _report.Datas
                    .GroupBy(q => q.Variant, new SSVariantDtoComparer())
                    .Select(q => (Variant: q.Key, Count: q.Select(d => d.SupplierId).Distinct().Count()))
                    .ToList();
                
                var itemsPerVariantSectionCount = itemsCountByVariants.Sum(q => q.Count > 1 ? q.Count + 1 : 1);

                var colQuantityStart = 8;
                var colPriceStart = colQuantityStart + suppliersCount + 1;
                var colTotalPriceStart = colPriceStart + suppliersCount + 1;
                var colVariantQuantityStart = colTotalPriceStart + suppliersCount + 1;
                var colVariantQuantityEnd = colVariantQuantityStart + itemsPerVariantSectionCount - 1;
                var colVariantTotalPriceStart = colVariantQuantityStart + itemsPerVariantSectionCount + 1;
                var colVariantTotalPriceEnd = colVariantTotalPriceStart + itemsPerVariantSectionCount - 1;

                var rowDataStart = 6;
                var rowTotal = rowDataStart + orderedCustomerItems.Count;

                ws.Column(1).Width = 1.33;
                ws.Column(2).AutoFit();
                ws.Column(3).Width = 14.67;
                ws.Column(4).Width = 27.67;
                ws.Column(5).Width = 10;
                ws.Column(6).Width = 10;
                ws.Column(colQuantityStart - 1).Width = 0.73; // separator 1
                ws.Column(colPriceStart - 1).Width = 0.73; // separator 2
                ws.Column(colTotalPriceStart - 1).Width = 0.73; // separator 3
                ws.Column(colVariantQuantityStart - 1).Width = 0.73; // separator 4
                ws.Column(colVariantTotalPriceStart - 1).Width = 0.73; // separator 5

                ws.Cells[1, 2].HeaderText("Анализ коммерческих предложений, приведенных к единицам измерения запроса")
                    .AlignLeft().NoWrapText();
                ws.Cells[2, 2].Value = $"Конкурентный лист № {_report.CLNumber} от {_report.CLCreatedOn:dd.MM.yyyy HH:mm}";

                ws.Cells[4, 2].Value = $"Клиент: {_report.Customer.Name}";
                ws.Cells[4, 4].Value = $"Заявка № {_report.Customer.PRNumber} от {_report.Customer.PRCreatedOn:dd.MM.yyyy HH:mm}";

                
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
                        ws.Cells[6 + nomPos, col].TableText(item.ConvertedQuantity);
                    }
                }

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
                        ws.Cells[6 + nomPos, col].TableText(item.ConvertedPrice);
                    }
                }

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
                        ws.Cells[6 + nomPos, col].TableText(item.ConvertedPrice * item.ConvertedQuantity);
                    }
                    ws.Cells[rowTotal, col].TableText(supplierItems.Sum(item => item.ConvertedPrice * item.ConvertedQuantity)).BoldFont();
                }

                #region Варианты распределения количества по поставщикам

                ws.Cells[2, colVariantQuantityStart].HeaderText("Варианты распределения количества по поставщикам").AlignLeft();
                ws.Cells[2, colVariantQuantityStart, 2, colVariantQuantityEnd].Merge = true;
                ws.Cells[3, colVariantQuantityStart].TableText("ЕИ запроса").AlignLeft();
                ws.Cells[rowTotal + 1, colVariantQuantityEnd].TableText("Доля поставщика в ИТОГО").AlignRight().GrayColor();

                foreach (var itemsCountPerVariant in itemsCountByVariants.OrderBy(q => q.Variant.Number))
                {
                    var variant = itemsCountPerVariant.Variant;
                    var colSectionStart = colVariantQuantityStart;

                    var previousVariants =
                        itemsCountByVariants.Where(q => q.Variant.Number < variant.Number).ToList();

                    if (previousVariants.Any())
                    {
                        colSectionStart += previousVariants.Sum(q => q.Count > 1 ? q.Count + 1 : 1);
                    }

                    var sectionWidth = itemsCountPerVariant.Count == 1 ? 1 : itemsCountPerVariant.Count + 1;
                    var colSectionEnd = colSectionStart + sectionWidth - 1;

                    ws.Cells[4, colSectionStart].HeaderText($"Вариант {variant.Number}").AlignLeft();
                    ws.Cells[4, colSectionStart, 4, colSectionEnd].Merge = true;

                    var variantData = datasByVariants.Find(q => q.Key.Id == variant.Id);
                    var variantSuppliersCount = variantData.Select(q => q.SupplierId).Distinct().Count();
                    var variantSuppliers = variantData.Select(q => q.Supplier).Distinct(new SSSupplierDtoComparer()).OrderBy(q => q.SONumber).ToList();

                    foreach (var item in orderedCustomerItems)
                    {
                        var itemIndex = orderedCustomerItems.IndexOf(item);
                        var itemRow = rowDataStart + itemIndex;

                        var datas = variantData.Where(q => q.NomenclatureId == item.NomenclatureId).ToList();

                        foreach (var data in datas)
                        {
                            var supplierIndex = variantSuppliers.IndexOf(variantSuppliers.Find(q => q.Id == data.SupplierId));
                            ws.Cells[itemRow, colSectionStart + supplierIndex].TableText(data.Quantity);
                        }
                        
                        if (variantSuppliersCount > 1)
                        {
                            var sumStartAddr = ws.Cells[itemRow, colSectionStart];
                            var sumEndAddr = ws.Cells[itemRow, colSectionEnd - 1];
                            ws.Cells[itemRow, colSectionEnd].Sum(sumStartAddr, sumEndAddr).BoldFont();
                        }
                    }
                    
                    foreach (var variantSupplier in variantSuppliers)
                    {
                        var supplierIndex = variantSuppliers.IndexOf(variantSupplier);
                        var colSupplier = colSectionStart + supplierIndex;
                        ws.Cells[rowDataStart - 1, colSupplier].HeaderText(variantSupplier.Name);
                        var sumCellStart = ws.Cells[rowDataStart, colSupplier];
                        var sumCellEnd = ws.Cells[rowTotal - 1, colSupplier];
                        var sumCellResult = ws.Cells[rowTotal, colSupplier];
                        sumCellResult.Sum(sumCellStart, sumCellEnd);
                        if (variantSuppliers.Count == 1)
                        {
                            sumCellResult.BoldFont();
                        }
                    }

                    if (variantSuppliers.Count > 1)
                    {
                        ws.Cells[rowDataStart - 1, colSectionEnd].HeaderText("ИТОГО");
                        var finalSumStartAddr = ws.Cells[rowTotal, colSectionStart];
                        var finalSumEndAddr = ws.Cells[rowTotal, colSectionEnd - 1];
                        ws.Cells[rowTotal, colSectionEnd].Sum(finalSumStartAddr, finalSumEndAddr).BoldFont();
                    }
                }

                #endregion

                #region Варианты распределения стоимости по поставщикам

                ws.Cells[2, colVariantTotalPriceStart].HeaderText("Варианты распределения стоимости по поставщикам").AlignLeft();
                ws.Cells[2, colVariantTotalPriceStart, 2, colVariantTotalPriceEnd].Merge = true;
                ws.Cells[3, colVariantTotalPriceStart].TableText("RUB").AlignLeft(); // todo: currency

                foreach (var itemsCountPerVariant in itemsCountByVariants.OrderBy(q => q.Variant.Number))
                {
                    var variant = itemsCountPerVariant.Variant;
                    var colSectionStart = colVariantTotalPriceStart;

                    var previousVariants =
                        itemsCountByVariants.Where(q => q.Variant.Number < variant.Number).ToList();

                    if (previousVariants.Any())
                    {
                        colSectionStart += previousVariants.Sum(q => q.Count > 1 ? q.Count + 1 : 1);
                    }

                    var sectionWidth = itemsCountPerVariant.Count == 1 ? 1 : itemsCountPerVariant.Count + 1;
                    var colSectionEnd = colSectionStart + sectionWidth - 1;

                    ws.Cells[4, colSectionStart].HeaderText($"Вариант {variant.Number}").AlignLeft();
                    ws.Cells[4, colSectionStart, 4, colSectionEnd].Merge = true;

                    var variantData = datasByVariants.Find(q => q.Key.Id == variant.Id);
                    var variantSuppliersCount = variantData.Select(q => q.SupplierId).Distinct().Count();
                    var variantSuppliers = variantData.Select(q => q.Supplier).Distinct(new SSSupplierDtoComparer()).OrderBy(q => q.SONumber).ToList();

                    foreach (var item in orderedCustomerItems)
                    {
                        var itemIndex = orderedCustomerItems.IndexOf(item);
                        var itemRow = rowDataStart + itemIndex;

                        var datas = variantData.Where(q => q.NomenclatureId == item.NomenclatureId).ToList();

                        foreach (var data in datas)
                        {
                            var supplierIndex = variantSuppliers.IndexOf(variantSuppliers.Find(q => q.Id == data.SupplierId));
                            ws.Cells[itemRow, colSectionStart + supplierIndex].TableText(data.Quantity*data.Price);
                        }

                        if (variantSuppliersCount > 1)
                        {
                            var sumStartAddr = ws.Cells[itemRow, colSectionStart];
                            var sumEndAddr = ws.Cells[itemRow, colSectionEnd - 1];
                            ws.Cells[itemRow, colSectionEnd].Sum(sumStartAddr, sumEndAddr).BoldFont();
                        }
                    }

                    foreach (var variantSupplier in variantSuppliers)
                    {
                        var supplierIndex = variantSuppliers.IndexOf(variantSupplier);
                        var colSupplier = colSectionStart + supplierIndex;
                        ws.Cells[rowDataStart - 1, colSupplier].HeaderText(variantSupplier.Name);
                        var sumCellStart = ws.Cells[rowDataStart, colSupplier];
                        var sumCellEnd = ws.Cells[rowTotal - 1, colSupplier];
                        var sumCellResult = ws.Cells[rowTotal, colSupplier];
                        sumCellResult.Sum(sumCellStart, sumCellEnd);
                        if (variantSuppliers.Count == 1)
                        {
                            sumCellResult.BoldFont();
                            ws.Cells[rowTotal + 1, colSupplier].Percentage(1).GrayColor().BoldFont();
                        }
                        else
                        {
                            var valueCell = ws.Cells[rowTotal, colSupplier];
                            var totalCell = ws.Cells[rowTotal, colSectionEnd];
                            ws.Cells[rowTotal + 1, colSupplier].Percentage(totalCell, valueCell).GrayColor();
                        }
                    }

                    if (variantSuppliers.Count > 1)
                    {
                        ws.Cells[rowDataStart - 1, colSectionEnd].HeaderText("ИТОГО");
                        var finalSumStartAddr = ws.Cells[rowTotal, colSectionStart];
                        var finalSumEndAddr = ws.Cells[rowTotal, colSectionEnd - 1];
                        ws.Cells[rowTotal, colSectionEnd].Sum(finalSumStartAddr, finalSumEndAddr).BoldFont();
                        ws.Cells[rowTotal + 1, colSectionEnd].Percentage(1).GrayColor().BoldFont();
                    }
                }

                #endregion
                
                // request items

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
                ws.Cells[dataRow, 4].TableText(_report.SelectedVariantNumber.ToString());
                ws.Cells[dataRow, 5].TableText(_report.SelectedVariantTotalPrice);
                ws.Cells[dataRow, 6].TableText("RUB"); // todo: currency
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
        private const string MoneyFormat = "### ### ##0.00";
        private const string PercentageFormat = "#0%";

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
                cell.Style.Numberformat.Format = MoneyFormat;
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
                cell.Style.Numberformat.Format = MoneyFormat;
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

        public static ExcelRange GrayColor(this ExcelRange cell)
        {
            cell.Style.Font.Color.SetColor(1, 128, 128, 128);
            return cell;
        }

        public static ExcelRange Sum(this ExcelRange cell, ExcelRange start, ExcelRange end)
        {
            cell.Formula = $"=SUM({start.Address}:{end.Address})";
            cell.Style.Numberformat.Format = MoneyFormat;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            return cell;
        }

        public static ExcelRange Percentage(this ExcelRange cell, ExcelRange total, ExcelRange value)
        {
            cell.Formula = $"={value.Address}/{total.Address}";
            cell.Style.Numberformat.Format = PercentageFormat;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            return cell;
        }

        public static ExcelRange Percentage(this ExcelRange cell, decimal value)
        {
            cell.Value = value;
            cell.Style.Numberformat.Format = PercentageFormat;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            return cell;
        }
    }
}

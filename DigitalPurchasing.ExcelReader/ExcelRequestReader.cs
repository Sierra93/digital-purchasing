using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using OfficeOpenXml;

namespace DigitalPurchasing.ExcelReader
{
    public class ExcelRequestReader : IExcelRequestReader
    {
        private const string CantFindColumnWithName = "Не удается найти таблицу в файле. Пожалуйста добавьте название колонки 'Наименование' в разделе 'Соответсвия названий колонок'";
        private const string CantOpenFile = "Не удается открыть файл";

        private readonly IColumnNameService _columnNameService;
        private ExcelWorksheet _ws;
        private string _allCells;

        public ExcelRequestReader(IColumnNameService columnNameService) => _columnNameService = columnNameService;

        public ExcelTableResponse ToTable(string filePath)
        {
            var result = new ExcelTable();

            var ext = Path.GetExtension(filePath).ToLower();
            if (ext == ".xls")
            {
                //todo: convert
            }

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                try
                {
                    _ws = GetDefaultWorksheet(package);
                }
                catch (Exception)
                {
                    return ExcelTableResponse.Error(CantOpenFile);
                }

                _allCells = _ws.Dimension.Address;

                var nameCount = -1;
                var nameAddr = SearchHeader(false, _columnNameService.GetNames(TableColumnType.Name)) ?? SearchHeader(true, "Наименование", "Name");
                if (nameAddr != null)
                {
                    var values = GetValuesForHeader(nameAddr, true);
                    result.Columns.Add(new ExcelTableColumn
                    {
                        Type = TableColumnType.Name,
                        Header = GetHeaderValue(nameAddr),
                        Values = values
                    });
                    nameCount = values.Count;
                }

                if (nameAddr == null) return ExcelTableResponse.Error(CantFindColumnWithName);

                var codeAddr = SearchHeader(false, _columnNameService.GetNames(TableColumnType.Code));
                if (codeAddr != null)
                {
                    result.Columns.Add(new ExcelTableColumn
                    {
                        Type = TableColumnType.Code,
                        Header = GetHeaderValue(codeAddr),
                        Values = GetValuesForHeader(codeAddr)
                    });
                }

                var uomAddr = SearchHeader(false, _columnNameService.GetNames(TableColumnType.Uom));
                if (uomAddr != null)
                {
                    result.Columns.Add(new ExcelTableColumn
                    {
                        Type = TableColumnType.Uom,
                        Header = GetHeaderValue(uomAddr),
                        Values = GetValuesForHeader(uomAddr)
                    });
                }

                var qtyAddr = SearchHeader(false, _columnNameService.GetNames(TableColumnType.Qty));
                if (qtyAddr != null)
                {
                    result.Columns.Add(new ExcelTableColumn
                    {
                        Type = TableColumnType.Qty,
                        Header = GetHeaderValue(qtyAddr),
                        Values = GetValuesForHeader(qtyAddr)
                    });
                }

                var priceAddr = SearchHeader(false, _columnNameService.GetNames(TableColumnType.Price));
                if (priceAddr != null)
                {
                    result.Columns.Add(new ExcelTableColumn
                    {
                        Type = TableColumnType.Price,
                        Header = GetHeaderValue(priceAddr),
                        Values = GetValuesForHeader(priceAddr)
                    });
                }

                var foundHeaderAddresses = new List<string> { nameAddr.Address, codeAddr?.Address, uomAddr?.Address, qtyAddr?.Address, priceAddr?.Address };

                // can't recognize table structure
                if (foundHeaderAddresses.Count == 0)
                {
                    return null;
                }

                var headerRows = new List<int?> { nameAddr.Row, codeAddr?.Row, uomAddr?.Row, qtyAddr?.Row, priceAddr?.Row };

                var uniqueHeaderRows = headerRows.Where(q => q.HasValue).Select(q => q.Value).Distinct().ToList();
                if (uniqueHeaderRows.Count > 1)
                {
                    // todo: check count of each row and select max?
                }
                else
                {
                    var otherHeaders = _ws.Cells[uniqueHeaderRows[0], _ws.Dimension.Start.Column, uniqueHeaderRows[0], _ws.Dimension.End.Column]
                        .Where(q => !string.IsNullOrEmpty(q.GetValue<string>()) && !foundHeaderAddresses.Contains(q.Address))
                        .Select(q => q.Start)
                        .ToList();

                    foreach (var otherHeader in otherHeaders)
                    {
                        var headerName = _ws.Cells[otherHeader.Address].GetValue<string>();
                        var headerValues = GetValuesForHeader(otherHeader);
                        result.Columns.Add(new ExcelTableColumn
                        {
                            Type = TableColumnType.Unknown,
                            Header = headerName,
                            Values = headerValues
                        });
                    }
                }

                if (nameCount != -1)
                {
                    foreach (var column in result.Columns)
                    {
                        column.Values = column.Values.Take(nameCount).ToList();
                    }
                }

                return ExcelTableResponse.Success(result);
            }
        }

        private ExcelWorksheet GetDefaultWorksheet(ExcelPackage package) => _ws = package.Workbook.Worksheets.First();

        private ExcelCellAddress SearchHeader(bool partialMatch = false, params string[] columnNames)
        {
            if (!columnNames.Any()) return null;

            var addresses = _ws
                .Cells[_allCells]
                .Where(q => !string.IsNullOrEmpty(q.GetValue<string>()) && columnNames.Contains(q.GetValue<string>(), StringComparer.InvariantCultureIgnoreCase))
                .Select(q => q.Start)
                .ToList();

            if (partialMatch && addresses.Count == 0)
            {
                addresses = _ws.Cells[_allCells]
                    .Where(q => !string.IsNullOrEmpty(q.GetValue<string>()) && columnNames.Any(w => q.GetValue<string>().Contains(w, StringComparison.InvariantCultureIgnoreCase)))
                    .Select(q => q.Start)
                    .ToList();
            }

            return addresses.Count == 1 ? addresses[0] : null;
        }

        private string GetHeaderValue(ExcelCellAddress address) => _ws.Cells[address.Address].GetValue<string>();

        private List<string> GetValuesForHeader(ExcelCellAddress address, bool trimLastEmpty = false)
        {
            if (address == null) return null;

            var strAddress = address.Address;
            var start = $"{strAddress[0]}{address.Row + 1}";
            var end = strAddress[0].ToString();

            var values = _ws.Cells[$"{start}:{end}"].Select(q => q.GetValue<string>()).ToList();

            if (trimLastEmpty)
            {
                while (values.Any() && string.IsNullOrEmpty(values.Last()))
                {
                    values.RemoveAt(values.Count - 1);
                }
            }

            return values;
        }
    }
}

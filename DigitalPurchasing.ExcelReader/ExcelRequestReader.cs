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
        private ExcelPackage _package;
        private ExcelWorksheet _ws;
        private string _allCells;

        public ExcelTable ToTable(string filePath)
        {
            var result = new ExcelTable();

            var ext = Path.GetExtension(filePath).ToLower();
            if (ext == ".xls")
            {
                //todo: convert
            }

            _package = new ExcelPackage(new FileInfo(filePath));
            _ws = GetDefaultWorksheet();
            _allCells = _ws.Dimension.Address;
            
            var idAddr = SearchHeader("#", "№");
            if (idAddr != null)
            {
                result.Columns.Add(new ExcelTableColumn
                {
                    Type = TableColumnType.Id,
                    Header = GetHeaderValue(idAddr),
                    Values = GetValuesForHeader(idAddr)
                });
            }

            var codeAddr = SearchHeader("Code", "Код");
            if (codeAddr != null)
            {
                result.Columns.Add(new ExcelTableColumn
                {
                    Type = TableColumnType.Code,
                    Header = GetHeaderValue(codeAddr),
                    Values = GetValuesForHeader(codeAddr)
                });
            }

            var nameCount = -1;
            var nameAddr = SearchHeader("Название", "Наименование", "Name", "Наименование товара");
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

            var uomAddr = SearchHeader("Ед.изм.", "Единица измерения", "ЕИ");
            if (uomAddr != null)
            {
                result.Columns.Add(new ExcelTableColumn
                {
                    Type = TableColumnType.Uom,
                    Header = GetHeaderValue(uomAddr),
                    Values = GetValuesForHeader(uomAddr)
                });
            }

            var qtyAddr = SearchHeader("Количество", "Кол-во", "Q-ty", "Quantity", "Qty");
            if (qtyAddr != null)
            {
                result.Columns.Add(new ExcelTableColumn
                {
                    Type = TableColumnType.Qty,
                    Header = GetHeaderValue(qtyAddr),
                    Values = GetValuesForHeader(qtyAddr)
                });
            }

            var foundHeaderAddresses = new List<string>
                { idAddr?.Address, codeAddr?.Address, nameAddr?.Address, uomAddr?.Address, qtyAddr?.Address };
            var headerRows = new List<int?> { idAddr?.Row, codeAddr?.Row, nameAddr?.Row, uomAddr?.Row, qtyAddr?.Row };
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

            return result;
        }

        private ExcelWorksheet GetDefaultWorksheet() => _ws = _package.Workbook.Worksheets.First();

        private ExcelCellAddress SearchHeader(params string[] columnNames)
        {
            var addresses = _ws
                .Cells[_allCells]
                .Where(q => columnNames.Contains(q.GetValue<string>(), StringComparer.InvariantCultureIgnoreCase))
                .Select(q => q.Start)
                .ToList();

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

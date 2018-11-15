using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using OfficeOpenXml;

namespace DigitalPurchasing.ExcelReader
{
    internal class TempColumnData
    {
        public TableColumnType Type { get; set; }
        public ExcelCellAddress HeaderAddr { get; set; }

        public TempColumnData(TableColumnType type, ExcelCellAddress headerAddr)
        {
            Type = type;
            HeaderAddr = headerAddr;
        }
    }

    public class ExcelRequestReader : IExcelRequestReader
    {
        private const string CantFindColumnWithName = "Не удается найти таблицу в файле. Пожалуйста добавьте название колонки 'Наименование' в разделе 'Соответсвия названий колонок'";
        private const string CantOpenFile = "Не удается открыть файл";

        private readonly IColumnNameService _columnNameService;

        public ExcelRequestReader(IColumnNameService columnNameService) => _columnNameService = columnNameService;

        public ExcelTableResponse ToTable(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            if (ext == ".xls")
            {
                // todo: convert file
                //filePath = Path.GetTempFileName()+".xlsx";
                return ExcelTableResponse.Error(CantOpenFile);
            }

            var result = new ExcelTable();
            
            using (var package = new ExcelPackage(new FileInfo(path)))
            {
                ExcelWorksheet ws;
                try
                {
                    ws = GetDefaultWorksheet(package);
                }
                catch (Exception)
                {
                    return ExcelTableResponse.Error(CantOpenFile);
                }

                var tempColumnDatas = new List<TempColumnData>();

                var defaultNameAddr = SearchHeaderAddresses(ws, true, "Наименование", "Name");
                var nameAddr = SearchHeaderAddresses(ws, false, _columnNameService.GetNames(TableColumnType.Name));

                if (defaultNameAddr.Count == 0 && nameAddr.Count == 0) return ExcelTableResponse.Error(CantFindColumnWithName);

                var codeAddr = SearchHeaderAddresses(ws, false, _columnNameService.GetNames(TableColumnType.Code));
                var uomAddr = SearchHeaderAddresses(ws, false, _columnNameService.GetNames(TableColumnType.Uom));
                var qtyAddr = SearchHeaderAddresses(ws, false, _columnNameService.GetNames(TableColumnType.Qty));
                var priceAddr = SearchHeaderAddresses(ws, false, _columnNameService.GetNames(TableColumnType.Price));

                tempColumnDatas.AddRange(defaultNameAddr.Select(q => new TempColumnData(TableColumnType.Name, q)));
                tempColumnDatas.AddRange(nameAddr.Select(q => new TempColumnData(TableColumnType.Name, q)));
                tempColumnDatas.AddRange(codeAddr.Select(q => new TempColumnData(TableColumnType.Code, q)));
                tempColumnDatas.AddRange(uomAddr.Select(q => new TempColumnData(TableColumnType.Uom, q)));
                tempColumnDatas.AddRange(qtyAddr.Select(q => new TempColumnData(TableColumnType.Qty, q)));
                tempColumnDatas.AddRange(priceAddr.Select(q => new TempColumnData(TableColumnType.Price, q)));

                var allKnownAddr = defaultNameAddr.Union(nameAddr).Union(codeAddr).Union(uomAddr).Union(qtyAddr).Union(priceAddr).ToList();

                var addrRows = allKnownAddr.Select(q => q.Row).Distinct().ToList();
                
                var headerRow = 0;
                if (addrRows.Count > 1)
                {
                    headerRow = addrRows
                        .GroupBy(q => q)
                        .Select(g => (Key: g.Key, Count: g.Count()))
                        .OrderByDescending(q => q.Count)
                        .First()
                        .Key;
                }
                else
                {
                    headerRow = addrRows[0];
                }

                var otherHeaderAddr = SearchOtherHeaderAddresses(ws, headerRow, allKnownAddr);
                var allAddr = allKnownAddr.Union(otherHeaderAddr).ToList();

                var valueAddr = SearchValueAddresses(ws, defaultNameAddr.Union(nameAddr).First());
                var valueRow = valueAddr[0].Row;

                var allColumns = allAddr.Union(valueAddr).Select(q => q.Column).Distinct().OrderBy(q => q).ToList();

                foreach (var column in allColumns)
                {
                    var addr = new ExcelCellAddress(headerRow, column);
                    if (tempColumnDatas.All(q => q.HeaderAddr.Address != addr.Address))
                    {
                        tempColumnDatas.Add(new TempColumnData(TableColumnType.Unknown, addr));
                    }
                }

                foreach (var tempColumnData in tempColumnDatas)
                {
                    var header = ws.Cells[tempColumnData.HeaderAddr.Address].Text;
                    var values = SearchValues(ws, tempColumnData.HeaderAddr, valueRow, tempColumnData.Type == TableColumnType.Name);
                    AddColumn(result.Columns, tempColumnData.Type, header, values);
                }

                var nameValuesCount = result.Columns.First(q => q.Type == TableColumnType.Name).Values.Count;
                foreach (var column in result.Columns)
                {
                    if (column.Values.Count > nameValuesCount)
                    {
                        column.Values = column.Values.Take(nameValuesCount).ToList();
                    }
                }

            }

            return ExcelTableResponse.Success(result);
        }

        private ExcelWorksheet GetDefaultWorksheet(ExcelPackage package) => package.Workbook.Worksheets.First();

        private List<ExcelCellAddress> SearchHeaderAddresses(ExcelWorksheet ws, bool partialMatch = false, params string[] columnNames)
        {
            if (!columnNames.Any()) return new List<ExcelCellAddress>();

            var addresses = ws
                .Cells[ws.Dimension.Address]
                .Where(q => !string.IsNullOrEmpty(q.Text) && columnNames.Contains(q.Text, StringComparer.InvariantCultureIgnoreCase))
                .Select(q => q.Start)
                .ToList();

            if (partialMatch && addresses.Count == 0)
            {
                addresses = ws.Cells[ws.Dimension.Address]
                    .Where(q => !string.IsNullOrEmpty(q.Text) && columnNames.Any(w => q.Text.Contains(w, StringComparison.InvariantCultureIgnoreCase)))
                    .Select(q => q.Start)
                    .ToList();
            }

            return addresses;
        }

        private List<ExcelCellAddress> SearchOtherHeaderAddresses(ExcelWorksheet ws, int row, IEnumerable<ExcelCellAddress> headerAddresses)
        {
            var strHeaderAddresses = headerAddresses.Select(q => q.Address).ToList();
            var addresses = ws.Cells[row, ws.Dimension.Start.Column, row, ws.Dimension.End.Column]
                .Where(q => !string.IsNullOrEmpty(q.Text) && !strHeaderAddresses.Contains(q.Address))
                .Select(q => q.Start)
                .ToList();
            return addresses;
        }

        private List<ExcelCellAddress> SearchValueAddresses(ExcelWorksheet ws, ExcelCellAddress headerAddr)
        {
            var firstValueAddr = ws.Cells[headerAddr.Row+1, headerAddr.Column, ws.Dimension.End.Row, headerAddr.Column]
                .First(q => !string.IsNullOrEmpty(q.Text))
                .Start;

            return SearchOtherHeaderAddresses(ws, firstValueAddr.Row, new List<ExcelCellAddress>());
        }

        private void AddColumn(List<ExcelTableColumn> columns, TableColumnType type, string header, List<string> values)
        {
            if (values.All(string.IsNullOrEmpty)) return; // skip empty columns

            if (string.IsNullOrEmpty(header))
            {
                header = "Без названия";
            }

            var count = columns.Count(q => q.Header.Equals(header, StringComparison.InvariantCultureIgnoreCase));
            if (count > 0)
            {
                header += $" ({count})";
            }
            columns.Add(new ExcelTableColumn { Type = type, Header = header, Values = values });
        }

        public List<string> SearchValues(ExcelWorksheet ws, ExcelCellAddress headerAddr, int valueRow, bool trimLastEmpty = false)
        {
            var values = ws.Cells[valueRow, headerAddr.Column, ws.Dimension.End.Row, headerAddr.Column]
                .Select(q => q.Text)
                .ToList();

            if (trimLastEmpty && values.Any())
            {
                var emptyIdx = values.FindIndex(string.IsNullOrEmpty);
                if (emptyIdx >= 0)
                {
                    var count = values.Count - emptyIdx;
                    values.RemoveRange(emptyIdx, count);
                }
            }

            return values;
        }
    }
}

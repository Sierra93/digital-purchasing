using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IExcelRequestReader
    {
        ExcelTable ToTable(string filePath);
    }

    public enum TableColumnType
    {
        Unknown = 0,
        Code = 20,
        Name = 30,
        Qty = 40,
        Uom = 50
    }

    public class ExcelTable
    {
        public List<ExcelTableColumn> Columns { get; set; } = new List<ExcelTableColumn>();

        public List<string> GetValues(TableColumnType type)
        {
            var column = Columns.FirstOrDefault(q => q.Type == type);
            return column?.Values;
        }

        public string GetValue(TableColumnType type, int index)
        {
            var values = GetValues(type);
            return values?[index];
        }

        public decimal GetDecimalValue(TableColumnType type, int index)
        {
            var value = GetValue(type, index);
            return value != null ? decimal.Parse(value) : 0;
        }

        public List<string> GetValues(string header)
        {
            var column = Columns.FirstOrDefault(q => q.Header == header);
            return column?.Values;
        }

        public string GetValue(string header, int index)
        {
            var values = GetValues(header);
            return values?[index];
        }

        public decimal GetDecimalValue(string header, int index)
        {
            var value = GetValue(header, index);
            return value != null ? decimal.Parse(value) : 0;
        }
    }

    public class ExcelTableColumn
    {
        public string Header { get; set; }
        public TableColumnType Type { get; set; }
        public List<string> Values { get; set; } = new List<string>();
    }
}

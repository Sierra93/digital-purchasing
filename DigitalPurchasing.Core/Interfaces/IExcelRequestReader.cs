using System;
using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core.Enums;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IExcelRequestReader
    {
        ExcelTableResponse ToTable(string filePath, Guid ownerId);
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
            if (values == null || index >= values.Count) return string.Empty;
            return values?[index];
        }

        public decimal GetDecimalValue(string header, int index)
        {
            var value = GetValue(header, index);
            if (string.IsNullOrEmpty(value)) return 0;
            return decimal.TryParse(value, out var result) ? result : 0;
        }
    }

    public class ExcelTableColumn
    {
        public string Header { get; set; }
        public TableColumnType Type { get; set; }
        public List<string> Values { get; set; } = new List<string>();
    }

    public class ExcelTableResponse
    {
        public bool IsSuccess { get; set; }
        public ExcelTable Table { get; set; }
        public string Message { get; set; }

        public string InvoiceData { get; set; }

        public static ExcelTableResponse Success(ExcelTable table, string invoiceData = null) => new ExcelTableResponse { Table = table, IsSuccess = true, InvoiceData = invoiceData };
        public static ExcelTableResponse Error(string message) => new ExcelTableResponse { IsSuccess = false, Message = message };
    }
}

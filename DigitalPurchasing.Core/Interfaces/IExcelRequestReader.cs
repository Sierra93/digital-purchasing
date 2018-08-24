using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IExcelRequestReader
    {
        ExcelTable ToTable(string filePath);
    }

    public enum TableColumnType
    {
        Unknown,
        Id,
        Code,
        Name,
        Quantity,
        UnitOfMeasurement
    }

    public class ExcelTable
    {
        public List<ExcelTableColumn> Columns { get; set; } = new List<ExcelTableColumn>();
    }

    public class ExcelTableColumn
    {
        public string Header { get; set; }
        public TableColumnType Type { get; set; }
        public List<string> Values { get; set; } = new List<string>();
    }
}

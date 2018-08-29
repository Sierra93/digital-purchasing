using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IExcelRequestReader
    {
        ExcelTable ToTable(string filePath);
    }

    public enum TableColumnType
    {
        Unknown = 0,
        Id = 10,
        Code = 20,
        Name = 30,
        Qty = 40,
        Uom = 50,
        Date = 60,
        Receiver = 70
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

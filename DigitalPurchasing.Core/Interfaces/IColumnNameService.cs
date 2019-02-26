using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IColumnNameService
    {
        string[] GetNames(TableColumnType type, Guid ownerId);
        void SaveName(TableColumnType type, string name, Guid ownerId);
        void SaveAllNames(ColumnResponse model);
        ColumnResponse GetAllNames();
    }

    public class ColumnResponse
    {
        public class Column
        {
            public string Name { get; set; }
            public List<string> AltNames { get; set; }
        }

        public List<Column> Columns = new List<Column>();

        public void AddColumn(string name, string[] altNames) => Columns.Add(new Column { Name = name, AltNames = altNames.ToList() });
    }
}

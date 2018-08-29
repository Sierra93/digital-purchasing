using System.Collections.Generic;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IColumnNameService
    {
        string[] GetNames(TableColumnType type);
        void SaveName(TableColumnType type, string name);
    }
}

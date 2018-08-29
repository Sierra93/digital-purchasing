using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Models
{
    public class ColumnName : BaseModelWithOwner
    {
        public string Names { get; set; }
        public TableColumnType Type { get; set; }
    }
}

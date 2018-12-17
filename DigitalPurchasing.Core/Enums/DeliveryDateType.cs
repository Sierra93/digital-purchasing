using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DigitalPurchasing.Core;

namespace DigitalPurchasing.Analysis2.Enums
{
    public enum DeliveryDateTerms
    {
        [Order(0), Description("Нет требований")]
        Any = 0,
        [Order(1), Description("Минимальный")]
        Min = 1,
        [Order(2), Description("Не позднее срока в заявке")]
        LessThanInRequest = 2
    }
}

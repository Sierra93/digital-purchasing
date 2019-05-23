using System.ComponentModel;

namespace DigitalPurchasing.Core.Enums
{
    public enum PaymentTerms
    {
        [Description("Нет требований")]
        NoRequirements = 0,
        [Description("Предоплата")]
        Prepay = 10,
        [Description("Отсрочка платежа")]
        Postponement = 20
    }
}

using System.ComponentModel.DataAnnotations;

namespace DigitalPurchasing.Core.Enums
{
    public enum ClientType
    {
        [Display(Name="Клиент")]
        Customer = 0,
        [Display(Name="Поставщик")]
        Supplier = 1
    }
}

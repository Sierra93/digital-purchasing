using System.ComponentModel.DataAnnotations;

namespace DigitalPurchasing.Core.Enums
{
    public enum SendPriceReductionTo: byte
    {
        [Display(Name="Поставщикам, предложившим минимальную цену")]
        MinPrice = 0,
        [Display(Name="Всем, от кого было получено КП")]
        All = 1
    }
}

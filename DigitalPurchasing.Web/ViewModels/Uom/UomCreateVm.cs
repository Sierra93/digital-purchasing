using System.ComponentModel.DataAnnotations;

namespace DigitalPurchasing.Web.ViewModels.Uom
{
    public class UomCreateVm
    {
        [Required, Display(Name = "Название")]
        public string Name { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace DigitalPurchasing.Web.ViewModels
{
    public class UomCreateVm
    {
        [Required]
        public string Name { get; set; }
    }
}

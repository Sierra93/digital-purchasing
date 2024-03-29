using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPurchasing.Web.ViewModels.Customer
{
    public class CustomerEditVm
    {
        public Guid Id { get; set; }

        [Display(Name = "Код клиента в Системе")]
        public int PublicId { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
    }
}

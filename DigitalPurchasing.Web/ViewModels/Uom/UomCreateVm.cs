using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DigitalPurchasing.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DigitalPurchasing.Web.ViewModels.Uom
{
    public class UomCreateVm
    {
        [Required, Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Альтернативные названия")]
        public List<string> AlternativeNames { get; set; }
    }
}

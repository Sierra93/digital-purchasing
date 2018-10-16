using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DigitalPurchasing.Web.ViewModels.NomenclatureCategory
{
    public class NomenclatureCategoryCreateVm
    {
        [Required, Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Родитель")]
        public Guid? ParentId { get; set; }

        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}

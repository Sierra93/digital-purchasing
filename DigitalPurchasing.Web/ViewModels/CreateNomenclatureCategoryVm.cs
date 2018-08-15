using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DigitalPurchasing.Web.ViewModels
{
    public class CreateNomenclatureCategoryVm
    {
        [Required]
        public string Name { get; set; }

        public Guid? ParentId { get; set; }

        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DigitalPurchasing.Web.Areas.Identity.Pages.Account.Manage
{
    [AuthorizeCompanyOwner]
    public class DefaultUomModel : PageModel
    {
        public class InputModel
        {
            [Required]
            [Display(Name = "Упаковка")]
            public Guid PackagingUomId { get; set; }
            public string PackagingUomName { get; set; }

            [Required]
            [Display(Name = "Масса")]
            public Guid MassUomId { get; set; }
            public string MassUomName { get; set; }

            [Required]
            [Display(Name = "Название ресурса")]
            public Guid ResourceUomId { get; set; }
            public string ResourceUomName { get; set; }

            [Required]
            [Display(Name = "ЕИ ресурса")]
            public Guid ResourceBatchUomId { get; set; }
            public string ResourceBatchUomName { get; set; }
        }

        private readonly IUomService _uomService;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public List<SelectListItem> EmptyList { get; set; } = new List<SelectListItem>();

        public DefaultUomModel(IUomService uomService)
            => _uomService = uomService;

        public async Task OnGetAsync()
        {
            var companyId = User.CompanyId();
            Input.PackagingUomId = await _uomService.GetPackagingUom(companyId);
            Input.MassUomId = await _uomService.GetMassUom(companyId);
            Input.ResourceUomId = await _uomService.GetResourceUom(companyId);
            Input.ResourceBatchUomId = await _uomService.GetResourceBatchUom(companyId);
            if (Input.PackagingUomId != Guid.Empty)
            {
                Input.PackagingUomName = _uomService.AutocompleteSingle(Input.PackagingUomId).Data.Name;
            }
            if (Input.MassUomId != Guid.Empty)
            {
                Input.MassUomName = _uomService.AutocompleteSingle(Input.MassUomId).Data.Name;
            }
            if (Input.ResourceUomId != Guid.Empty)
            {
                Input.ResourceUomName = _uomService.AutocompleteSingle(Input.ResourceUomId).Data.Name;
            }
            if (Input.ResourceBatchUomId != Guid.Empty)
            {
                Input.ResourceBatchUomName = _uomService.AutocompleteSingle(Input.ResourceBatchUomId).Data.Name;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var companyId = User.CompanyId();
            await _uomService.SetPackagingUom(companyId, Input.PackagingUomId);
            await _uomService.SetMassUom(companyId, Input.MassUomId);
            await _uomService.SetResourceUom(companyId, Input.ResourceUomId);
            await _uomService.SetResourceBatchUom(companyId, Input.ResourceBatchUomId);
            StatusMessage = "Изменения сохранены";
            return RedirectToPage();
        }
    }
}

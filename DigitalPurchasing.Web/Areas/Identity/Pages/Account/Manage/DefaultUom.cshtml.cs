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
        }

        private readonly IUomService _uomService;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public List<SelectListItem> UomsList { get; set; } = new List<SelectListItem>();

        public DefaultUomModel(IUomService uomService)
        {
            _uomService = uomService;
            UomsList = _uomService.GetAll()
                .OrderBy(q => q.Name)
                .Select(q => new SelectListItem(q.Name, q.Id.ToString()))
                .ToList();
        }

        public async Task OnGetAsync()
        {
            var companyId = User.CompanyId();
            Input.PackagingUomId = await _uomService.GetPackagingUom(companyId);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var companyId = User.CompanyId();
            await _uomService.SetPackagingUom(companyId, Input.PackagingUomId);
            StatusMessage = "Изменения сохранены";
            return Page();
        }
    }
}

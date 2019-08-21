using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models.Identity;
using DigitalPurchasing.Web.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalPurchasing.Web.Areas.Identity.Pages.Account.Manage
{
    [AuthorizeCompanyOwner]
    public class CompanyModel : PageModel
    {
        private readonly ICompanyService _companyService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public CompanyModel(
            ICompanyService companyService,
            SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            _companyService = companyService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required, Display(Name = "Название")]
            public string CompanyName { get; set; }

            [Display(Name = "Разрешить удаление КП пользователями")]
            public bool IsSODeleteEnabled { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var company = await _companyService.GetById(User.CompanyId());

            Input = new InputModel
            {
                CompanyName = company.Name,
                IsSODeleteEnabled = company.IsSODeleteEnabled
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            
            var companyId = User.CompanyId();

            await _companyService.Update(new CompanyDto
            {
                Id = companyId,
                IsSODeleteEnabled = Input.IsSODeleteEnabled,
                Name = Input.CompanyName
            });

            StatusMessage = "Изменения сохранены";
            return RedirectToPage();
        }
    }
}

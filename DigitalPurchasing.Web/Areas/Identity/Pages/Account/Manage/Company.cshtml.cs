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
        }

        [Display(Name = "Ссылка для приглашения")]
        public string InvitationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Input = new InputModel
            {
                CompanyName = _companyService.GetByUser(User.Id()).Name
            };

            var invitationCode = await _companyService.GetInvitationCode(User.CompanyId());
            InvitationUrl = Url.Action("Index", "Invite", new { code = invitationCode }, Request.Scheme);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _companyService.UpdateName(user.Id, Input.CompanyName);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Изменения сохранены";
            return RedirectToPage();
        }
    }
}

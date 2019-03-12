using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DigitalPurchasing.Web.Areas.Identity.Pages.Account.Manage
{
    [AuthorizeCompanyOwner]
    public class UsersModel : PageModel
    {
        private readonly ICompanyService _companyService;

        public UsersModel(ICompanyService companyService)
            => _companyService = companyService;

        [TempData]
        public string StatusMessage { get; set; }

        public List<CompanyUserDto> Users { get; set; }

        [Display(Name = "Ссылка для приглашения сотрудников")]
        public string InvitationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Users = await _companyService.GetCompanyUsers(User.CompanyId());
            var invitationCode = await _companyService.GetInvitationCode(User.CompanyId());
            InvitationUrl = Url.Action("Index", "Invite", new { code = invitationCode }, Request.Scheme);
            return Page();
        }
    }
}

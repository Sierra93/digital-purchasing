using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    [Route("i")]
    public class InviteController : Controller
    {
        private readonly ICompanyService _companyService;

        public InviteController(ICompanyService companyService)
            => _companyService = companyService;

        [Route("{code}")]
        public async Task<IActionResult> Index(string code)
        {
            var isValidCode = await _companyService.IsValidInvitationCode(code);
            if (!isValidCode)
            {
                return NotFound();
            }

            return RedirectToPage("/Account/Register", new { code, area = "Identity" });
        }
    }
}

using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    [AuthorizeAdmin]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
            => _adminService = adminService;

        public async Task<IActionResult> Index()
        {
            var vm = await _adminService.GetDashboard();
            return View(vm);
        }

        public async Task<IActionResult> Companies()
        {
            var vm = await _adminService.GetCompanies();
            return View(vm);
        }
    }
}

using System.IO;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    [AuthorizeAdmin]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IFileService _fileService;

        public AdminController(
            IAdminService adminService,
            IFileService fileService)
        {
            _adminService = adminService;
            _fileService = fileService;
        }

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

        [HttpPost]
        public async Task<IActionResult> UploadTerms(IFormFile file)
        {
            if (file == null)
            {
                return RedirectToAction(nameof(Index));
            }

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                _fileService.CreateTermsFile(file.FileName, ms.ToArray(), file.ContentType);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DownloadTerms()
        {
            var termsFile = _fileService.GetTermsFile();

            if (termsFile == null) return NotFound();

            return File(termsFile.Bytes, termsFile.ContentType, termsFile.FileName);
        }
    }
}

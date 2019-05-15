using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DigitalPurchasing.Web.Models;
using DigitalPurchasing.Core.Interfaces;
using Microsoft.Net.Http.Headers;

namespace DigitalPurchasing.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFileService _fileService;

        public HomeController(
            IFileService fileService)
        {
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        public IActionResult Oferta()
        {
            var termsFile = _fileService.GetTermsFile();

            if (termsFile == null) return NotFound();

            Response.Headers.Add(HeaderNames.ContentDisposition, new System.Net.Mime.ContentDisposition
            {
                FileName = "Oferta.pdf",
                Inline = true
            }.ToString());
            return File(termsFile.Bytes, termsFile.ContentType);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

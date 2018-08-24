using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class PurchasingRequestController : BaseController
    {
        private readonly IPurchasingRequestService _purchasingRequestService;

        public PurchasingRequestController(IPurchasingRequestService purchasingRequestService) => _purchasingRequestService = purchasingRequestService;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Edit(Guid id)
        {
            var response = _purchasingRequestService.GetById(id);
            if (response == null) return NotFound();

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var fileName = file.FileName;
            var fileExt = Path.GetExtension(fileName);
            var filePath = Path.GetTempFileName()+fileExt;

            using (var output = System.IO.File.Create(filePath))
                await file.CopyToAsync(output);

            var id = _purchasingRequestService.CreateFromFile(filePath);

            return RedirectToAction(nameof(Edit), new { id });
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models.Identity;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.PurchasingRequest;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public partial class PurchaseRequestController : BaseController
    {
        private readonly IPurchaseRequestService _purchasingRequestService;
        private readonly INomenclatureService _nomenclatureService;
        private readonly ICompanyService _companyService;
        private readonly UserManager<User> _userManager;
        private readonly IUomService _uomService;

        public PurchaseRequestController(
            IPurchaseRequestService purchasingRequestService,
            INomenclatureService nomenclatureService,
            ICompanyService companyService,
            UserManager<User> userManager,  
            IUomService uomService)
        {
            _purchasingRequestService = purchasingRequestService;
            _nomenclatureService = nomenclatureService;
            _companyService = companyService;
            _userManager = userManager;
            _uomService = uomService;
        }

        public IActionResult Edit(Guid id)
        {
            var response = _purchasingRequestService.GetById(id);
            if (response == null) return NotFound();

            if (response.Status == PurchaseRequestStatus.MatchColumns)
            {
                return View("EditMatchColumns", response);
            }

            if (response.Status == PurchaseRequestStatus.ManualInput)
            {
                return View("EditManual", response);
            }

            if (response.Status == PurchaseRequestStatus.MatchItems)
            {
                return View("EditMatchItems", response);
            }

            return NotFound();
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

            var ownerId = User.CompanyId();

            var response = await _purchasingRequestService.CreateFromFile(filePath, ownerId);
            if (!response.IsSuccess)
            {
                TempData["Message"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Edit), new { id = response.Id });
        }

        [HttpPost]
        public IActionResult Delete([FromBody]DeleteVm vm)
        {
            var result = _purchasingRequestService.Delete(vm.Id);
            return Ok(result);
        }
    }
}

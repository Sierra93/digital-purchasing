using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    public partial class PurchasingRequestController : BaseController
    {
        private readonly IPurchasingRequestService _purchasingRequestService;
        private readonly INomenclatureService _nomenclatureService;
        private readonly ICompanyService _companyService;
        private readonly UserManager<User> _userManager;
        private readonly IUomService _uomService;

        public PurchasingRequestController(
            IPurchasingRequestService purchasingRequestService,
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

            if (response.Status == PurchasingRequestStatus.MatchColumns)
            {
                return View("EditMatchColumns", response);
            }

            if (response.Status == PurchasingRequestStatus.ManualInput)
            {
                return View("EditManual", response);
            }

            if (response.Status == PurchasingRequestStatus.MatchItems)
            {
                return View("EditMatchItems", response);
            }

            return NotFound();
        }

        public IActionResult EditManual(Guid id)
        {
            var response = _purchasingRequestService.GetById(id);
            if (response == null) return NotFound();

            return View(response);
        }
        
        [HttpGet]
        public IActionResult RawItemsData(Guid id)
        {
            var response = _purchasingRequestService.GetRawItems(id);
            return Json(response);
        }

        [HttpPost]
        public IActionResult SaveRawItemsData([FromBody] SaveRawItemsDataVm model)
        {
            var id = model.PurchasingRequestId;
            var userId = Guid.Parse(_userManager.GetUserId(User));
            _purchasingRequestService.SaveCompanyName(id, _companyService.GetByUser(userId).Name);
            _purchasingRequestService.SaveRawItems(id, model.Items);
            _purchasingRequestService.UpdateStatus(id, PurchasingRequestStatus.MatchItems);
            return Ok();
        }

        [HttpPost]
        public IActionResult SaveCustomerName([FromBody] SaveCustomerNameVm model)
        {
            _purchasingRequestService.SaveCustomerName(model.Id, model.Name);
            return Ok();
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

            var response = _purchasingRequestService.CreateFromFile(filePath);
            if (!response.IsSuccess)
            {
                TempData["Message"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Edit), new { id = response.Id });
        }
    }

   
}

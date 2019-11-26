using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models.Identity;
using DigitalPurchasing.Services;
using DigitalPurchasing.Web.ViewModels;
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
        private readonly INomenclatureAlternativeService _nomenclatureAlternativeService;
        private readonly INomenclatureCategoryService _nomenclatureCategoryService;
        private readonly IPurchaseRequestAttachmentService _purchaseRequestAttachmentService;

        public PurchaseRequestController(
            IPurchaseRequestService purchasingRequestService,
            INomenclatureService nomenclatureService,
            ICompanyService companyService,
            UserManager<User> userManager,  
            IUomService uomService,
            INomenclatureAlternativeService nomenclatureAlternativeService,
            INomenclatureCategoryService nomenclatureCategoryService,
            IPurchaseRequestAttachmentService purchaseRequestAttachmentService)
        {
            _purchasingRequestService = purchasingRequestService;
            _nomenclatureService = nomenclatureService;
            _companyService = companyService;
            _userManager = userManager;
            _uomService = uomService;
            _nomenclatureAlternativeService = nomenclatureAlternativeService;
            _nomenclatureCategoryService = nomenclatureCategoryService;
            _purchaseRequestAttachmentService = purchaseRequestAttachmentService;
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

        [HttpPost]
        public async Task<IActionResult> UploadAttachments(IFormCollection formCollection)
        {
            var id = Guid.Parse(formCollection["id"]);

            var allowedExts = new List<string>
            {
                ".pdf", ".xls", ".xlsx", ".png", ".jpg", ".jpeg", ".doc", ".docx"
            };
            
            foreach (var formFile in formCollection.Files)
            {
                var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();
                
                if (allowedExts.Contains(ext) && formFile.Length > 0)
                {
                    await _purchaseRequestAttachmentService.SaveAttachmentAsync(id, formFile.OpenReadStream(), formFile.FileName);
                }
            }

            var attachments = await _purchaseRequestAttachmentService.GetAttachmentsAsync(id);

            return Ok(attachments);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAttachment([FromBody]DeleteVm vm)
        {
            await _purchaseRequestAttachmentService.DeleteAttachment(vm.Id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> AttachmentsData([FromQuery] Guid id)
        {
            var attachments = await _purchaseRequestAttachmentService.GetAttachmentsAsync(id);
            return Ok(attachments);
        }
    }
}

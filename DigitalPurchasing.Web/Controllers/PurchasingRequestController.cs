using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.PurchasingRequest;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class PurchasingRequestController : BaseController
    {
        private readonly IPurchasingRequestService _purchasingRequestService;

        public PurchasingRequestController(IPurchasingRequestService purchasingRequestService) => _purchasingRequestService = purchasingRequestService;

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Data(VueTableRequest request)
        {
            var result = _purchasingRequestService.GetData(request.Page, request.PerPage, request.SortField, request.SortAsc);
            var nextUrl = Url.Action("Data", "PurchasingRequest", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("Data", "PurchasingRequest", request.PrevPageRequest(), Request.Scheme);
            var data = result.Data.Adapt<List<PurchasingRequestDataVm>>();
            foreach (var d in data)
            {
                d.EditUrl = Url.Action("Edit", new { id = d.Id });
            }
            return Json(new VueTableResponse<PurchasingRequestDataVm>(data, request, result.Total, nextUrl, prevUrl));
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

        public IActionResult ColumnsData(Guid id)
        {
            var response = _purchasingRequestService.GetColumnsById(id);
            return Json(response);
        }

        [HttpPost]
        public IActionResult SaveColumnsData([FromBody]SavePurchasingRequestColumnsVm model)
        {
            var id = model.PurchasingRequestId;
            _purchasingRequestService.SaveColumns(id, model);
            _purchasingRequestService.GenerateRawItems(id);
            _purchasingRequestService.UpdateStatus(id, PurchasingRequestStatus.ManualInput);
            return Ok();
        }

        public IActionResult RawItemsData(Guid id)
        {
            var response = _purchasingRequestService.GetRawItems(id);
            return Json(response);
        }

        [HttpPost]
        public IActionResult SaveRawItemsData([FromBody] SaveRawItemsDataVm model)
        {
            var id = model.PurchasingRequestId;
            _purchasingRequestService.SaveRawItems(id, model.Items);
            _purchasingRequestService.UpdateStatus(id, PurchasingRequestStatus.MatchItems);
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.CompetitionList;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class CompetitionListController : BaseController
    {
        private readonly ICompetitionListService _competitionListService;
        private readonly ISupplierOfferService _supplierOfferService;

        public CompetitionListController(ICompetitionListService competitionListService, ISupplierOfferService supplierOfferService)
        {
            _competitionListService = competitionListService;
            _supplierOfferService = supplierOfferService;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult IndexData(VueTableRequest request)
        {
            var nextUrl = Url.Action("IndexData", "CompetitionList", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("IndexData", "CompetitionList", request.PrevPageRequest(), Request.Scheme);

            var result = _competitionListService.GetData(request.Page, request.PerPage, request.SortField, request.SortAsc);

            var data = result.Data
                .Adapt<List<CompetitionListIndexDataItemVm>>()
                .Select(q => { q.EditUrl = Url.Action(nameof(Edit), new { id = q.Id }); return q; })
                .ToList();

            return Json(new VueTableResponse<CompetitionListIndexDataItemVm, VueTableRequest>(data, request, result.Total, nextUrl, prevUrl));
        }

        public async Task<IActionResult> Create([FromQuery]Guid qrId)
        {
            var id = await _competitionListService.GetIdByQR(qrId, false);
            if (id == Guid.Empty) return NotFound();
            return RedirectToAction(nameof(Edit), new { id });
        }

        public IActionResult Edit(Guid id)
        {
            var vm = _competitionListService.GetById(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromRoute]Guid id, IFormFile file)
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

            var response = await _supplierOfferService.CreateFromFile(id, filePath);
            if (!response.IsSuccess)
            {
                TempData["Message"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Edit), "SupplierOffer", new { id = response.Id });
        }

        [HttpPost]
        public IActionResult Delete([FromBody]DeleteVm vm)
        {
            var result = _competitionListService.Delete(vm.Id);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult LoadSOTerms([FromQuery]Guid? soId)
        {
            if (soId.HasValue)
            {
                return Ok(_supplierOfferService.GetTerms(soId.Value));
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult SaveSOTerms([FromBody]SoTermsVm req, [FromQuery]Guid? soId)
        {
            if (req == null || !soId.HasValue) return NotFound();

            _supplierOfferService.SaveTerms(req, soId.Value);
            return Ok();
        }
    }
}

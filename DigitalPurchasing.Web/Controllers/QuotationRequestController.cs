using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.ExcelReader;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.QuotationRequest;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DigitalPurchasing.Web.Controllers
{
    public class QuotationRequestController : BaseController
    {
        public class SentRequestsModel
        {
            [JsonProperty("id")]
            public Guid QuotationRequestId { get; set; }
            public List<Guid> Suppliers { get; set; }
        }

        private readonly IQuotationRequestService _quotationRequestService;

        public QuotationRequestController(IQuotationRequestService quotationRequestService)
            => _quotationRequestService = quotationRequestService;

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Data(VueTableRequest request)
        {
            var nextUrl = Url.Action("Data", "QuotationRequest", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("Data", "QuotationRequest", request.PrevPageRequest(), Request.Scheme);

            var result = _quotationRequestService.GetData(request.Page, request.PerPage, request.SortField, request.SortAsc);

            var data = result.Data
                .Adapt<List<QuotationRequestIndexDataItemVm>>()
                .Select(q => { q.EditUrl = Url.Action(nameof(View), new { id = q.Id }); return q; })
                .ToList();

            return Json(new VueTableResponse<QuotationRequestIndexDataItemVm, VueTableRequest>(data, request, result.Total, nextUrl, prevUrl));
        }

        public IActionResult Create([FromQuery]Guid prId)
        {
            var id = _quotationRequestService.GetQuotationRequestId(prId);
            if (id == Guid.Empty) return NotFound();
            return RedirectToAction(nameof(View), new { id });
        }

        public IActionResult View(Guid id)
        {
            var model = _quotationRequestService.GetById(id);
            return View(model);
        }

        public IActionResult ViewDData([FromQuery]Guid qrId)
        {
            var model = _quotationRequestService.GetViewData(qrId);
            return Ok(model);
        }

        public IActionResult Download([FromQuery]Guid qrId)
        {
            var qr = _quotationRequestService.GetById(qrId);
            var bytes = _quotationRequestService.GenerateExcelForQR(qrId);
            var filename = $"RFQ_{qr.CreatedOn:yyyyMMdd}_{qr.PublicId}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }

        [HttpPost]
        public IActionResult Delete([FromBody]DeleteVm vm)
        {
            var result = _quotationRequestService.Delete(vm.Id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SentRequests([FromBody] SentRequestsModel model)
        {

            await _quotationRequestService.SendRequests(User.Id(), model.QuotationRequestId, model.Suppliers);
            var sentRequests = _quotationRequestService.GetSentRequests(model.QuotationRequestId);
            return Ok(sentRequests);
        }
    }
}

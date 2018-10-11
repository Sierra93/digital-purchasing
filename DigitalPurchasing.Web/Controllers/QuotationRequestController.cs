using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels.QuotationRequest;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class QuotationRequestController : Controller
    {
        private readonly IQuotationRequestService _quotationRequestService;
        private readonly IPurchaseRequestService _purchaseRequestService;

        public QuotationRequestController(IQuotationRequestService quotationRequestService, IPurchaseRequestService purchaseRequestService)
        {
            _quotationRequestService = quotationRequestService;
            _purchaseRequestService = purchaseRequestService;
        }

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

        public IActionResult CreateOrView([FromQuery]Guid purchaseRequestId)
        {
            var id = _quotationRequestService.GetQuotationRequestId(purchaseRequestId);
            if (id == Guid.Empty) return NotFound();
            return RedirectToAction(nameof(View), new { id });
        }

        public IActionResult View(Guid id)
        {
            var model = _quotationRequestService.GetById(id);
            return View(model);
        }

        public IActionResult ViewNomenclatureData([FromQuery]Guid qrId)
        {
            var model = _quotationRequestService.GetViewData(qrId);
            return Ok(model);
        }
    }
}

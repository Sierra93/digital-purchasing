using System.Collections.Generic;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public partial class PurchaseRequestController
    {
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Data(VueTableRequest request)
        {
            var result = _purchasingRequestService.GetData(request.Page, request.PerPage, request.SortField, request.SortAsc);
            var nextUrl = Url.Action("Data", "PurchaseRequest", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("Data", "PurchaseRequest", request.PrevPageRequest(), Request.Scheme);
            var data = result.Data.Adapt<List<PurchaseRequestDataVm>>();
            foreach (var d in data)
            {
                d.EditUrl = Url.Action("Edit", new { id = d.Id });
            }
            return Json(new VueTableResponse<PurchaseRequestDataVm, VueTableRequest>(data, request, result.Total, nextUrl, prevUrl));
        }
    }
}

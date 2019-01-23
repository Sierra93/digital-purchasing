using System.Collections.Generic;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService) => _supplierService = supplierService;

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Data(VueTableRequest request)
        {
            var result = _supplierService.GetData(request.Page, request.PerPage, request.SortField, request.SortAsc);
            var nextUrl = Url.Action(nameof(Data), "Supplier", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action(nameof(Data), "Supplier", request.PrevPageRequest(), Request.Scheme);
            return Json(new VueTableResponse<SupplierIndexDataItem, VueTableRequest>(result.Data, request, result.Total, nextUrl, prevUrl));
        }

        [HttpGet]
        public IActionResult Autocomplete([FromQuery] string q) => Json(_supplierService.Autocomplete(new AutocompleteOptions { Query = q }));
    }
}

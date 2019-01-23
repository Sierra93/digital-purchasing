using System;
using System.Collections.Generic;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.Supplier;
using Mapster;
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

            var dataItems = result.Data.Adapt<List<SupplierIndexDataItemEdit>>();
            foreach (var dataItem in dataItems)
            {
                dataItem.EditUrl = Url.Action(nameof(Edit), new { id = dataItem.Id });
            }

            return Json(new VueTableResponse<SupplierIndexDataItemEdit, VueTableRequest>(dataItems, request, result.Total, nextUrl, prevUrl));
        }

        [HttpGet]
        public IActionResult Autocomplete([FromQuery] string q) => Json(_supplierService.Autocomplete(new AutocompleteOptions { Query = q }));

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var vm = _supplierService.GetById(id);
            if (vm == null) return NotFound();
            return View(vm);
        }
    }
}

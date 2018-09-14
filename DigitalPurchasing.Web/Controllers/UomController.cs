using System;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Services;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class UomController : BaseController
    {
        private readonly IUomService _uomService;

        public UomController(IUomService uomService) => _uomService = uomService;

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Data(VueTableRequest request)
        {
            var result = _uomService.GetData(request.Page, request.PerPage, request.SortField, request.SortAsc);
            var nextUrl = Url.Action("Data", "Uom", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("Data", "Uom", request.PrevPageRequest(), Request.Scheme);

            return Json(new VueTableResponse<UomResult>(result.Data, request, result.Total, nextUrl, prevUrl));
        }

        public IActionResult Create() => View(new UomCreateVm());

        [HttpPost]
        public IActionResult Create(CreateNomenclatureCategoryVm vm)
        {
            if (ModelState.IsValid)
            {
                _uomService.CreateUom(vm.Name);
                return RedirectToAction("Index");
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult Autocomplete(string q)
        {
            var response = _uomService.Autocomplete(q);
            return Json(response);
        }

        [HttpGet]
        public IActionResult AutocompleteSingle([FromQuery] Guid id) => Json(_uomService.AutocompleteSingle(id));
    }
}

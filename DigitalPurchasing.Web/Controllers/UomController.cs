using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Services;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.NomenclatureCategory;
using DigitalPurchasing.Web.ViewModels.Uom;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DigitalPurchasing.Web.Controllers
{
    public class UomController : BaseController
    {
        private readonly IUomService _uomService;
        private readonly IConversionRateService _conversionRateService;

        public UomController(
            IUomService uomService,
            IConversionRateService conversionRateService)
        {
            _uomService = uomService;
            _conversionRateService = conversionRateService;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Data(VueTableRequest request)
        {
            var result = _uomService.GetData(request.Page, request.PerPage, request.SortField, request.SortAsc);
            var nextUrl = Url.Action("Data", "Uom", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("Data", "Uom", request.PrevPageRequest(), Request.Scheme);

            var data = result.Data.Adapt<List<UomIndexDataItemEdit>>();
            foreach (var dataItem in data)
            {
                dataItem.EditUrl = Url.Action("Edit", new { id = dataItem.Id });
            }

            return Json(new VueTableResponse<UomIndexDataItemEdit, VueTableRequest>(data, request, result.Total, nextUrl, prevUrl));
        }

        [HttpGet, Route("/uom/factordata/{id}")]
        public IActionResult FactorData(VueTableRequestWithId request)
        {
            var result = _uomService.GetFactorData(request.Id, request.Page, request.PerPage, request.SortField, request.SortAsc);
            var nextUrl = Url.Action("Data", "Uom", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("Data", "Uom", request.PrevPageRequest(), Request.Scheme);
            return Json(new VueTableResponse<UomFactorDataItem, VueTableRequestWithId>(result.Data, request, result.Total, nextUrl, prevUrl));
        }

        public IActionResult Create() => View(new UomCreateVm());

        [HttpPost]
        public IActionResult Create(UomCreateVm vm)
        {
            if (ModelState.IsValid)
            {
                var companyId = User.CompanyId();
                _uomService.Create(companyId, vm.Name, vm.AlternativeNames);
                return RedirectToAction("Index");
            }

            return View(vm);
        }

        public IActionResult Edit(Guid id)
        {
            var uom = _uomService.GetById(id);
            if (uom == null) return NotFound();
            var vm = uom.Adapt<UomEditVm>();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(UomEditVm vm)
        {
            if (ModelState.IsValid)
            {
                _uomService.Update(vm.Id, vm.Name, vm.AlternativeNames);
                return RedirectToAction("Index");
            }

            return View(vm);
        }

        public class EditDataVm
        {
            public List<string> AlternativeNames { get; set; }
        }

        public IActionResult EditData(Guid id)
        {
            var uom = _uomService.GetById(id);
            if (uom == null) return NotFound();
            var vm = new EditDataVm
            {
                AlternativeNames = uom.Json?.AlternativeNames.Select(q => q.Name).ToList() ?? new List<string>()
            };
            return Json(vm);
        }

        [HttpGet]
        public IActionResult Autocomplete(string q)
        {
            var response = _uomService.Autocomplete(q, User.CompanyId());
            return Json(response);
        }

        [HttpGet]
        public IActionResult AutocompleteSingle([FromQuery] Guid id) => Json(_uomService.AutocompleteSingle(id));

        [HttpPost]
        public async Task<IActionResult> Factor([FromBody]UomFactorVm vm)
        {
            var result = await _conversionRateService
                .GetRate(vm.FromId, vm.NomenclatureId, vm.CustomerId, vm.SupplierId);

            return Json(result);
        }

        [HttpPost]
        public IActionResult SaveFactor([FromBody]UomSaveFactorVm vm)
        {
            var companyId = User.CompanyId();
            _uomService.SaveConversionRate(
                companyId,
                vm.FromUomId,
                vm.ToUomId,
                vm.NomenclatureAlternativeId,
                vm.FactorC, vm.FactorN);
            return Ok();
        }

        [HttpPost]
        public IActionResult DeleteFactor([FromBody] UomDeleteFactorVm vm)
        {
            _uomService.DeleteConversionRate(vm.Id);
            return Ok();
        }

        [HttpPost]
        public IActionResult Delete([FromBody]DeleteVm vm)
        {
            _uomService.Delete(vm.Id);
            return Ok();
        }
    }
}

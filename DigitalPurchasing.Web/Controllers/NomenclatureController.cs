using System;
using System.Collections.Generic;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Services;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DigitalPurchasing.Web.Controllers
{
    public class NomenclatureController : BaseController
    {
        private readonly INomenclatureService _nomenclatureService;
        private readonly IDictionaryService _dictionaryService;

        public NomenclatureController(INomenclatureService nomenclatureService, IDictionaryService dictionaryService)
        {
            _nomenclatureService = nomenclatureService;
            _dictionaryService = dictionaryService;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Data(VueTableRequest request)
        {
            var result = _nomenclatureService.GetData(request.Page, request.PerPage, request.SortField, request.SortAsc);
            var nextUrl = Url.Action("Data", "Nomenclature", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("Data", "Nomenclature", request.PrevPageRequest(), Request.Scheme);
            var data = result.Data.Adapt<List<NomenclatureDataVm>>();
            foreach (var d in data)
            {
                d.EditUrl = Url.Action("Edit", new { id = d.Id });
            }
            return Json(new VueTableResponse<NomenclatureDataVm>(data, request, result.Total, nextUrl, prevUrl));
        }

        [HttpGet]
        public IActionResult Autocomplete([FromQuery] string q) => Json(_nomenclatureService.Autocomplete(q));

        [HttpGet]
        public IActionResult AutocompleteSingle([FromQuery] Guid id) => Json(_nomenclatureService.AutocompleteSingle(id));

        public IActionResult Create()
        {
            var vm = new NomenclatureCreateVm();
            vm.BatchUoms = vm.ResourceBatchUoms = vm.MassUoms = vm.ResourceUoms = _dictionaryService.GetUoms();
            vm.Categories = _dictionaryService.GetCategories();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(NomenclatureCreateVm vm)
        {
            if (ModelState.IsValid)
            {
                _nomenclatureService.Create(vm.Adapt<NomenclatureResult>());
                return RedirectToAction(nameof(Index));
            }

            vm.BatchUoms = vm.ResourceBatchUoms = vm.MassUoms = vm.ResourceUoms = _dictionaryService.GetUoms();
            vm.Categories = _dictionaryService.GetCategories();

            return View(vm);
        }

        public IActionResult Edit(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            var data = _nomenclatureService.GetById(id);
            if (data == null) return NotFound();

            var vm = data.Adapt<NomenclatureCreateVm>();

            vm.BatchUoms = vm.ResourceBatchUoms = vm.MassUoms = vm.ResourceUoms = _dictionaryService.GetUoms();
            vm.Categories = _dictionaryService.GetCategories();

            return View(nameof(Create), vm);
        }

        [HttpPost]
        public IActionResult Edit(NomenclatureCreateVm vm)
        {
            if (ModelState.IsValid)
            {
                _nomenclatureService.Update(vm.Adapt<NomenclatureResult>());
                return RedirectToAction(nameof(Index));
            }

            vm.BatchUoms = vm.ResourceBatchUoms = vm.MassUoms = vm.ResourceUoms = _dictionaryService.GetUoms();
            vm.Categories = _dictionaryService.GetCategories();

            return View(nameof(Create), vm);
        }
    }
}

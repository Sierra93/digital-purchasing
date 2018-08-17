using System.Collections.Generic;
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
            return Json(new VueTableResponse<NomenclatureResult>(result.Data, request, result.Total, nextUrl, prevUrl));
        }

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
                return RedirectToAction("Index");
            }

            vm.BatchUoms = vm.ResourceBatchUoms = vm.MassUoms = vm.ResourceUoms = _dictionaryService.GetUoms();
            vm.Categories = _dictionaryService.GetCategories();

            return View(vm);
        }
    }
}

using System.Linq;
using DigitalPurchasing.Services;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DigitalPurchasing.Web.Controllers
{
    public class NomenclatureCategoryController : BaseController
    {
        private readonly INomenclatureCategoryService _nomenclatureCategoryService;
        private readonly IDictionaryService _dictionaryService;

        public NomenclatureCategoryController(INomenclatureCategoryService nomenclatureCategoryService, IDictionaryService dictionaryService)
        {
            _nomenclatureCategoryService = nomenclatureCategoryService;
            _dictionaryService = dictionaryService;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Data(VueTableRequest request)
        {
            var result = _nomenclatureCategoryService.GetData(request.Page, request.PerPage, request.SortField, request.SortAsc);
            var nextUrl = Url.Action("Data", "NomenclatureCategory", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("Data", "NomenclatureCategory", request.PrevPageRequest(), Request.Scheme);

            return Json(new VueTableResponse<NomenclatureCategoryResult>(result.Data, request, result.Total, nextUrl, prevUrl));
        }

        public IActionResult Create()
        {
            var vm = new CreateNomenclatureCategoryVm
            {
                Categories = _dictionaryService.GetCategories()
            };
            vm.Categories.Insert(0, new SelectListItem("", ""));
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(CreateNomenclatureCategoryVm vm)
        {
            if (ModelState.IsValid)
            {
                _nomenclatureCategoryService.CreateCategory(vm.Name, vm.ParentId);
                return RedirectToAction("Index");
            }

            vm.Categories = _dictionaryService.GetCategories();

            return View(vm);
        }
    }
}

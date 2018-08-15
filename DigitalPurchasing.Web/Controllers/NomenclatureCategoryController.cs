using System.Linq;
using DigitalPurchasing.Services;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DigitalPurchasing.Web.Controllers
{
    public class NomenclatureCategoryController : Controller
    {
        private readonly INomenclatureCategoryService _nomenclatureCategoryService;

        public NomenclatureCategoryController(INomenclatureCategoryService nomenclatureCategoryService) => _nomenclatureCategoryService = nomenclatureCategoryService;

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
                Categories = _nomenclatureCategoryService.GetAll().Select(q => new SelectListItem(q.Name, q.Id.ToString("N"))).ToList()
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

            return View(vm);
        }
    }
}

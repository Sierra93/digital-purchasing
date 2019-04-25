using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Services;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.NomenclatureCategory;
using Mapster;
using Microsoft.AspNetCore.Http;
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

            var data = result.Data.Adapt<List<NomenclatureCategoryIndexDataItemEdit>>();
            foreach (var dataItem in data)
            {
                dataItem.EditUrl = Url.Action("Edit", new { id = dataItem.Id });
            }

            return Json(new VueTableResponse<NomenclatureCategoryIndexDataItemEdit, VueTableRequest>(data, request, result.Total, nextUrl, prevUrl));
        }

        public IActionResult Create()
        {
            var vm = new NomenclatureCategoryCreateVm
            {
                Categories = _dictionaryService.GetCategories()
            };
            vm.Categories.Insert(0, new SelectListItem("", ""));
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(NomenclatureCategoryCreateVm vm)
        {
            if (ModelState.IsValid)
            {
                _nomenclatureCategoryService.CreateOrUpdate(vm.Name, vm.ParentId);
                return RedirectToAction("Index");
            }

            vm.Categories = _dictionaryService.GetCategories();

            return View(vm);
        }

        public IActionResult Edit(Guid id)
        {
            var category = _nomenclatureCategoryService.GetById(id);
            if (category == null) return NotFound();

            var vm = category.Adapt<NomenclatureCategoryEditVm>();
            vm.Categories = _dictionaryService.GetCategories();
            vm.Categories.Insert(0, new SelectListItem("", ""));
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(NomenclatureCategoryEditVm vm)
        {
            if (ModelState.IsValid)
            {
                _nomenclatureCategoryService.Update(vm.Id, vm.Name, vm.ParentId);
                return RedirectToAction("Index");
            }

            vm.Categories = _dictionaryService.GetCategories();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Delete([FromBody]DeleteVm vm)
        {
            _nomenclatureCategoryService.Delete(vm.Id);
            return Ok();
        }

        [HttpGet]
        public IActionResult Template()
        {
            var excelTemplate = new ExcelReader.NomenclatureCategoryListTemplate.ExcelTemplate();
            return File(excelTemplate.Build(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "template.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> UploadTemplate(IFormFile file)
        {
            if (file == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var fileName = file.FileName;
            var fileExt = Path.GetExtension(fileName);
            var filePath = Path.GetTempFileName() + fileExt;

            using (var output = System.IO.File.Create(filePath))
                await file.CopyToAsync(output);

            var excelTemplate = new ExcelReader.NomenclatureCategoryListTemplate.ExcelTemplate();

            var datas = excelTemplate.Read(filePath);

            Action<Guid?, Queue<string>> createCategoryHierarchy = null;
            createCategoryHierarchy = (Guid? parentCategoryId, Queue<string> nestedCategories) =>
            {
                if (nestedCategories.Any())
                {
                    var categoryName = nestedCategories.Dequeue();
                    if (!string.IsNullOrWhiteSpace(categoryName))
                    {
                        var category = _nomenclatureCategoryService.CreateOrUpdate(categoryName, parentCategoryId);
                        createCategoryHierarchy(category.Id, nestedCategories);
                    }
                }
            };

            foreach (var item in datas)
            {
                createCategoryHierarchy(null, new Queue<string>(new string[] { item.MainCategory, item.SubCategory1, item.SubCategory2 }));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

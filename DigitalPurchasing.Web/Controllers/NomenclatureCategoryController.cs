using System;
using System.Collections.Generic;
using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Services;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.NomenclatureCategory;
using Mapster;
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
    }
}

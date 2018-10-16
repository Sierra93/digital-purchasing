using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.ExcelReader;
using DigitalPurchasing.Services;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DigitalPurchasing.Web.Controllers
{
    public class NomenclatureController : BaseController
    {
        private readonly INomenclatureService _nomenclatureService;
        private readonly INomenclatureCategoryService _nomenclatureCategoryService;
        private readonly IDictionaryService _dictionaryService;
        private readonly IUomService _uomService;

        public NomenclatureController(
            INomenclatureService nomenclatureService,
            INomenclatureCategoryService nomenclatureCategoryService,
            IDictionaryService dictionaryService,
            IUomService uomService)
        {
            _nomenclatureService = nomenclatureService;
            _nomenclatureCategoryService = nomenclatureCategoryService;
            _dictionaryService = dictionaryService;
            _uomService = uomService;
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
            return Json(new VueTableResponse<NomenclatureDataVm, VueTableRequest>(data, request, result.Total, nextUrl, prevUrl));
        }

        [HttpGet, Route("/nomenclature/datadetails/{id}")]
        public IActionResult DataDetails(VueTableRequestWithId request)
        {
            var result = _nomenclatureService.GetDetailsData(request.Id, request.Page, request.PerPage, request.SortField, request.SortAsc);
            var nextUrl = Url.Action("DataDetails", "Nomenclature", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("DataDetails", "Nomenclature", request.PrevPageRequest(), Request.Scheme);

            return Json(new VueTableResponse<NomenclatureDetails, VueTableRequestWithId>(result.Data, request, result.Total, nextUrl, prevUrl));
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
                _nomenclatureService.CreateOrUpdate(vm.Adapt<NomenclatureResult>());
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

        [HttpPost]
        public IActionResult Delete([FromBody]DeleteVm vm)
        {
            _nomenclatureService.Delete(vm.Id);
            return Ok();
        }

        [HttpGet]
        public IActionResult Template()
        {
            var excelTemplate = new ExcelTemplate();
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
            var filePath = Path.GetTempFileName()+fileExt;

            using (var output = System.IO.File.Create(filePath))
                await file.CopyToAsync(output);

            var excelTemplate = new ExcelTemplate();

            var datas = excelTemplate.Read(filePath);

            var uoms = datas.Select(q => q.Uom);
            var uomsMass = datas.Select(q => q.UomMass);
            var uomsResource = datas.Select(q => q.ResourceUom);
            var uomsResourceBatch = datas.Select(q => q.ResourceBatchUom);
            var allUoms = uoms.Union(uomsMass).Union(uomsResource).Union(uomsResourceBatch).Distinct().ToList();

            var dbUoms = new Dictionary<string, Guid>();

            foreach (var uom in allUoms)
            {
                var result = _uomService.CreateOrUpdate(uom);
                if (!dbUoms.ContainsValue(result.Id))
                {
                    dbUoms.Add(result.Name, result.Id);
                }
            }

            var dbCategories = new Dictionary<string, Guid>();

            var allCategories = datas.Select(q => q.Category);
            foreach (var dataCategory in allCategories)
            {
                var categories = dataCategory.Split('>', StringSplitOptions.RemoveEmptyEntries);
                Guid? parentId = null;
                foreach (var category in categories)
                {
                    var result = _nomenclatureCategoryService.CreateOrUpdate(category, parentId);
                    parentId = result.Id;
                    if (!dbCategories.ContainsValue(result.Id))
                    {
                        dbCategories.Add(result.Name, result.Id);
                    }
                }
            }

            foreach (var data in datas)
            {
                var dataCategory = data.Category.Split('>', StringSplitOptions.RemoveEmptyEntries).Last();
                _nomenclatureService.CreateOrUpdate(new NomenclatureResult
                {
                    Name = data.Name,
                    NameEng =  data.NameEng,
                    Code = data.Code,
                    BatchUomId = dbUoms.First(q => q.Key.Equals(data.Uom, StringComparison.InvariantCultureIgnoreCase)).Value,
                    MassUomId = dbUoms.First(q => q.Key.Equals(data.UomMass, StringComparison.InvariantCultureIgnoreCase)).Value,
                    ResourceUomId = dbUoms.First(q => q.Key.Equals(data.ResourceUom, StringComparison.InvariantCultureIgnoreCase)).Value,
                    ResourceBatchUomId = dbUoms.First(q => q.Key.Equals(data.ResourceBatchUom, StringComparison.InvariantCultureIgnoreCase)).Value,
                    CategoryId = dbCategories.First(q => q.Key.Equals(dataCategory, StringComparison.InvariantCultureIgnoreCase)).Value,
                    MassUomValue = data.UomMassValue,
                    ResourceUomValue = data.ResourceUomValue
                });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

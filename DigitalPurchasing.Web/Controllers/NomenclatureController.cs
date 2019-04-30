using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.ExcelReader;
using DigitalPurchasing.Services.Exceptions;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.Nomenclature;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class NomenclatureController : BaseController
    {
        private readonly INomenclatureService _nomenclatureService;
        private readonly INomenclatureCategoryService _nomenclatureCategoryService;
        private readonly IDictionaryService _dictionaryService;
        private readonly IUomService _uomService;
        private const string SameNomenclatureNameErrorMessage = "Номенклатура с таким наименованием уже есть в системе";

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
            var result = _nomenclatureService.GetData(request.Page, request.PerPage, request.SortField, request.SortAsc, request.Search);
            var nextUrl = Url.Action("Data", "Nomenclature", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("Data", "Nomenclature", request.PrevPageRequest(), Request.Scheme);
            var data = result.Data.Adapt<List<NomenclatureIndexDataItemEdit>>();
            foreach (var d in data)
            {
                d.EditUrl = Url.Action("Edit", new { id = d.Id });
            }
            return Json(new VueTableResponse<NomenclatureIndexDataItemEdit, VueTableRequest>(data, request, result.Total, nextUrl, prevUrl));
        }

        [HttpGet, Route("/nomenclature/detailsdata/{id}")]
        public IActionResult DataDetails(VueTableRequestWithId request)
        {
            var result = _nomenclatureService.GetDetailsData(request.Id, request.Page, request.PerPage, request.SortField, request.SortAsc);
            var nextUrl = Url.Action("DataDetails", "Nomenclature", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action("DataDetails", "Nomenclature", request.PrevPageRequest(), Request.Scheme);
            var data = result.Data.Adapt<List<NomenclatureDetailsDataItemEdit>>();
            foreach (var d in data)
            {
                d.EditUrl = Url.Action("DetailsEdit", new { id = d.Id });
            }
            return Json(new VueTableResponse<NomenclatureDetailsDataItemEdit, VueTableRequestWithId>(data, request, result.Total, nextUrl, prevUrl));
        }

        [HttpGet]
        public IActionResult Autocomplete([FromQuery] string q)
        {
            return Json(_nomenclatureService.Autocomplete(new AutocompleteOptions { Query = q, OwnerId = User.CompanyId() }));
        }

        [HttpGet]
        public IActionResult AutocompleteSingle([FromQuery] Guid id) => Json(_nomenclatureService.AutocompleteSingle(id));

        public IActionResult Create()
        {
            var vm = new NomenclatureEditVm();
            LoadDictionaries(vm);
            return View(nameof(Edit), vm);
        }

        public IActionResult Edit(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            var data = _nomenclatureService.GetById(id);
            if (data == null) return NotFound();

            var vm = data.Adapt<NomenclatureEditVm>();

            LoadDictionaries(vm);

            return View(nameof(Edit), vm);
        }

        [HttpPost]
        public IActionResult Modify(NomenclatureEditVm vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _nomenclatureService.CreateOrUpdate(vm.Adapt<NomenclatureVm>());
                    return RedirectToAction(nameof(Index));
                }
                catch (SameNomenclatureNameException)
                {
                    ModelState.AddModelError(string.Empty, SameNomenclatureNameErrorMessage);
                }
            }

            LoadDictionaries(vm);

            return View(nameof(Edit), vm);
        }

        private void LoadDictionaries(NomenclatureEditVm vm)
        {
            vm.BatchUoms = vm.ResourceBatchUoms = vm.MassUoms = vm.ResourceUoms = _dictionaryService.GetUoms();
            vm.PackUoms = _dictionaryService.GetUoms().AddEmpty();
            vm.Categories = _dictionaryService.GetCategories();
        }

        public IActionResult DetailsEdit(Guid id)
        {
            if (id == Guid.Empty) return NotFound();
            var alt = _nomenclatureService.GetAlternativeById(id);
            if (alt == null) return NotFound();
            var vm = alt.Adapt<NomenclatureAlternativeEditVm>();
            vm.BatchUoms = vm.ResourceBatchUoms = vm.MassUoms = vm.ResourceUoms = _dictionaryService.GetUoms().AddEmpty();
            return View(vm);
        }

        [HttpPost]
        public IActionResult DetailsEdit(NomenclatureAlternativeEditVm vm)
        {
            if (ModelState.IsValid)
            {
                _nomenclatureService.UpdateAlternative(vm.Adapt<NomenclatureAlternativeVm>());
                return RedirectToAction(nameof(Index));
            }

            vm.BatchUoms = vm.ResourceBatchUoms = vm.MassUoms = vm.ResourceUoms = _dictionaryService.GetUoms().AddEmpty();
            return View(vm);
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
            var packUoms = datas.Select(q => q.PackUom);
            var allUoms = uoms.Union(uomsMass).Union(uomsResource).Union(uomsResourceBatch).Union(packUoms)
                .Where(q => !string.IsNullOrWhiteSpace(q)).Distinct().ToList();

            var dbUoms = new List<Tuple<string, Guid>>();

            foreach (var uom in allUoms)
            {
                var result = _uomService.CreateOrUpdate(uom);
                if (!dbUoms.Any(_ => _.Item2 == result.Id))
                {
                    dbUoms.Add(new Tuple<string, Guid>(uom, result.Id));
                }
            }

            var dbCategories = new Dictionary<Guid, string>();
            var allCategories = datas.Select(q => q.Category);
            foreach (var dataCategory in allCategories)
            {
                var categories = dataCategory.Split('>', StringSplitOptions.RemoveEmptyEntries);
                Guid? parentId = null;
                foreach (var category in categories)
                {
                    var result = _nomenclatureCategoryService.CreateOrUpdate(category, parentId);
                    parentId = result.Id;
                    if (!dbCategories.ContainsKey(result.Id))
                    {
                        dbCategories.Add(result.Id, result.Name);
                    }
                }
            }

            var el = new Tuple<string, Guid>("", Guid.NewGuid())?.Item2;

            var nomenclatures = datas.Select(data => new NomenclatureVm
            {
                Name = data.Name,
                NameEng =  data.NameEng,
                Code = data.Code,
                BatchUomId = dbUoms.First(q => q.Item1.Equals(data.Uom, StringComparison.InvariantCultureIgnoreCase)).Item2,
                MassUomId = dbUoms.First(q => q.Item1.Equals(data.UomMass, StringComparison.InvariantCultureIgnoreCase)).Item2,
                ResourceUomId = dbUoms.First(q => q.Item1.Equals(data.ResourceUom, StringComparison.InvariantCultureIgnoreCase)).Item2,
                ResourceBatchUomId = dbUoms.First(q => q.Item1.Equals(data.ResourceBatchUom, StringComparison.InvariantCultureIgnoreCase)).Item2,
                CategoryId = dbCategories.First(q => q.Value.Equals(
                    data.Category.Split('>', StringSplitOptions.RemoveEmptyEntries).Last(), StringComparison.InvariantCultureIgnoreCase)).Key,
                MassUomValue = data.UomMassValue,
                ResourceUomValue = data.ResourceUomValue,
                PackUomId = dbUoms.FirstOrDefault(q => q.Item1.Equals(data.PackUom, StringComparison.InvariantCultureIgnoreCase))?.Item2,
                PackUomValue = data.PackUomValue ?? 0
            }).GroupBy(q => q.Name).Select(q => q.First()).ToList();

            var companyId = User.CompanyId();

            _nomenclatureService.CreateOrUpdate(nomenclatures, companyId);

            return RedirectToAction(nameof(Index));
        }
    }
}

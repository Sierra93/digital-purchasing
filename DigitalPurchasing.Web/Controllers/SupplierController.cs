using System;
using System.Collections.Generic;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Services.Exceptions;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.Supplier;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Linq;
using DigitalPurchasing.Core.Extensions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using DigitalPurchasing.Data;

namespace DigitalPurchasing.Web.Controllers
{
    public class SupplierController : Controller
    {
        public class DeleteContactPersonPost
        {
            [JsonProperty("id")] public Guid PersonId { get; set; }
        }

        private readonly ISupplierService _supplierService;
        private readonly INomenclatureCategoryService _nomenclatureCategoryService;
        private readonly ApplicationDbContext _db;
        private const string SameInnErrorMessage = "Контрагент с таким ИНН уже есть в системе";

        public SupplierController(
            ISupplierService supplierService,
            INomenclatureCategoryService nomenclatureCategoryService,
            ApplicationDbContext db)
        {
            _supplierService = supplierService;
            _nomenclatureCategoryService = nomenclatureCategoryService;
            _db = db;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Data(VueTableRequest request)
        {
            var result = _supplierService.GetData(
                request.Page,
                request.PerPage,
                request.SortField,
                request.SortAsc,
                request.Search);

            var nextUrl = Url.Action(nameof(Data), "Supplier", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action(nameof(Data), "Supplier", request.PrevPageRequest(), Request.Scheme);

            var dataItems = result.Data.Adapt<List<SupplierIndexDataItemEdit>>();
            foreach (var dataItem in dataItems)
            {
                dataItem.EditUrl = Url.Action(nameof(Edit), new { id = dataItem.Id });
            }

            return Json(new VueTableResponse<SupplierIndexDataItemEdit, VueTableRequest>(dataItems, request, result.Total, nextUrl, prevUrl));
        }

        [HttpGet]
        public IActionResult Autocomplete([FromQuery] string q, [FromQuery] bool includeCategories = false)
            => Json(_supplierService.Autocomplete(new AutocompleteOptions { Query = q }, includeCategories));

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var data = _supplierService.GetById(id);
            if (data == null) return NotFound();

            var vm = new SupplierEditVm
            {
                Supplier = data.Adapt<SupplierEditVm.SupplierVm>()
            };

            LoadRelatedData(vm, id);

            return View(vm);
        }

        private void LoadRelatedData(SupplierEditVm vm, Guid? supplierId)
        {
            if (supplierId.HasValue)
            {
                vm.ContactPersons = _supplierService.GetContactPersonsBySupplier(supplierId.Value);
                var categories = _supplierService.GetSupplierNomenclatureCategories(supplierId.Value)
                    .OrderByDescending(_ => _.IsDefaultSupplierCategory).ToList();
                vm.NomenclatureCategoies = categories;
            }
            if (!vm.NomenclatureCategoies.Any(nc => nc.IsDefaultSupplierCategory))
            {
                vm.NomenclatureCategoies.Insert(0, new SupplierNomenclatureCategory()
                {
                    IsDefaultSupplierCategory = true,                    
                });
            }
            vm.AvailableCategories = _nomenclatureCategoryService.GetAll(true).ToList();
        }

        [HttpPost]
        public IActionResult Edit(SupplierEditVm vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _supplierService.Update(vm.Supplier.Adapt<SupplierVm>());
                    DeleteSupplierCategories(vm.Supplier.Id);
                    SaveSupplierCategories(vm, vm.Supplier.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (SameInnException)
                {
                    ModelState.AddModelError(string.Empty, SameInnErrorMessage);
                }
            }

            LoadRelatedData(vm, vm.Supplier.Id);
            return View(vm);
        }

        private void SaveSupplierCategories(SupplierEditVm vm, Guid supplierId)
            => _supplierService.SaveSupplierNomenclatureCategoryContacts(supplierId, vm.NomenclatureCategoies
                    .Where(nc => nc.NomenclatureCategoryId.HasValue)
                    .Select(nc => (
                        nc.NomenclatureCategoryId.Value,
                        nc.NomenclatureCategoryPrimaryContactId,
                        nc.NomenclatureCategorySecondaryContactId,
                        nc.IsDefaultSupplierCategory
                    )));

        private void DeleteSupplierCategories(Guid supplierId)
            => _supplierService.RemoveSupplierNomenclatureCategories(supplierId);

        [HttpGet, Route("/contactpersons/add/{supplierId}")]
        public IActionResult AddContactPerson([FromRoute]Guid supplierId)
        {
            var supplier = _supplierService.GetById(supplierId);
            if (supplier == null) return NotFound();

            var vm = new SupplierContactPersonEditVm
            {
                SupplierId = supplier.Id,
                SupplierName = supplier.Name
            };
            return View("EditContactPerson", vm);
        }

        [HttpGet, Route("/contactpersons/edit/{personId}")]
        public IActionResult EditContactPerson([FromRoute]Guid personId)
        {
            var person = _supplierService.GetContactPersonsById(personId);
            if (person == null) return NotFound();

            var supplier = _supplierService.GetById(person.SupplierId);

            var vm = person.Adapt<SupplierContactPersonEditVm>();
            vm.SupplierName = supplier.Name;
            return View("EditContactPerson",vm);
        }

        [HttpPost]
        public IActionResult SaveContactPerson(SupplierContactPersonEditVm vm)
        {
            if (!ModelState.IsValid)
            {
                return View("EditContactPerson", vm);
            }

            var supplier = _supplierService.GetById(vm.SupplierId);
            if (supplier == null) return BadRequest();

            var model = vm.Adapt<SupplierContactPersonVm>();
            if (vm.Id == Guid.Empty)
            {
                _supplierService.AddContactPerson(model);
            }
            else
            {
                _supplierService.EditContactPerson(model);
            }
            
            return RedirectToAction("Edit", new { id = vm.SupplierId });
        }

        [HttpPost]
        public IActionResult DeleteContactPerson([FromBody] DeleteContactPersonPost model)
        {
            if (model.PersonId != Guid.Empty)
            {
                _supplierService.DeleteContactPerson(model.PersonId);
            }

            return Ok();
        }

        public IActionResult Create()
        {
            var vm = new SupplierEditVm();
            LoadRelatedData(vm, null);
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(SupplierEditVm vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid supplierId = _supplierService.CreateSupplier(vm.Supplier.Adapt<SupplierVm>(), User.CompanyId());
                    SaveSupplierCategories(vm, supplierId);
                    return RedirectToAction(nameof(Index));
                }
                catch (SameInnException)
                {
                    ModelState.AddModelError(string.Empty, SameInnErrorMessage);
                }
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult Template()
        {
            var excelTemplate = new ExcelReader.SupplierListTemplate.ExcelTemplate();
            return File(excelTemplate.Build(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "template.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> UploadTemplate(IFormFile file)
        {
            if (file == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var companyId = User.CompanyId();

            var fileName = file.FileName;
            var fileExt = Path.GetExtension(fileName);
            var filePath = Path.GetTempFileName() + fileExt;

            using (var output = System.IO.File.Create(filePath))
                await file.CopyToAsync(output);

            var excelTemplate = new ExcelReader.SupplierListTemplate.ExcelTemplate();

            var datas = excelTemplate.Read(filePath);

            bool supplierWithSameInnExist = false;

            foreach (var item in datas.Where(_ => !string.IsNullOrWhiteSpace(_.SupplierName)))
            {
                try
                {
                    NomenclatureCategoryVm category = null;

                    if (!string.IsNullOrWhiteSpace(item.MainCategory))
                    {
                        category = _nomenclatureCategoryService.CreateOrUpdate(companyId, item.MainCategory, null);
                        if (!string.IsNullOrWhiteSpace(item.SubCategory1))
                        {
                            category = _nomenclatureCategoryService.CreateOrUpdate(companyId, item.SubCategory1, category.Id);
                            if (!string.IsNullOrWhiteSpace(item.SubCategory2))
                            {
                                category = _nomenclatureCategoryService.CreateOrUpdate(companyId, item.SubCategory2, category.Id);
                            }
                        }
                    }

                    var supplierId = _supplierService.CreateSupplier(new SupplierVm
                    {
                        Inn = item.Inn,
                        ErpCode = item.ErpCode,
                        Name = item.SupplierName,
                        OwnershipType = item.OwnershipType,
                        PriceWithVat = item.PriceWithVat,
                        Website = item.Website,
                        ActualAddressCity = item.ActualAddressCity,
                        ActualAddressCountry = item.ActualAddressCountry,
                        ActualAddressStreet = item.ActualAddressStreet,
                        LegalAddressCity = item.LegalAddressCity,
                        LegalAddressCountry = item.LegalAddressCountry,
                        LegalAddressStreet = item.LegalAddressStreet,
                        WarehouseAddressCity = item.WarehouseAddressCity,
                        WarehouseAddressCountry = item.WarehouseAddressCountry,
                        WarehouseAddressStreet = item.WarehouseAddressStreet,
                        DeliveryTerms = item.DeliveryTerms,
                        Note = item.Note,
                        OfferCurrency = item.OfferCurrency,
                        PaymentDeferredDays = item.PaymentDeferredDays,
                        Phone = item.SupplierPhone,
                        SupplierType = item.SupplierType,
                        CategoryId = category?.Id
                    }, User.CompanyId());

                    var mainContactId = item.ContactSpecified ? (Guid?)_supplierService.AddContactPerson(new SupplierContactPersonVm
                    {
                        SupplierId = supplierId,
                        Email = item.ContactEmail,
                        FirstName = item.ContactFirstName,
                        LastName = item.ContactLastName,
                        JobTitle = item.ContactJobTitle,
                        MobilePhoneNumber = item.ContactMobilePhone,
                        PhoneNumber = item.ContactPhone
                    }) : null;

                    if (mainContactId.HasValue && category != null)
                    {
                        _supplierService.SaveSupplierNomenclatureCategoryContacts(supplierId,
                            new List<(Guid NomenclatureCategoryId, Guid? PrimarySupplierContactId, Guid? SecondarySupplierContactId, bool IsDefaultSupplierCategory)>
                            {
                                (category.Id, mainContactId.Value, null, false)
                            });
                    }
                }
                catch (SameInnException)
                {
                    supplierWithSameInnExist = true;
                }                
            }

            if (supplierWithSameInnExist)
            {
                TempData["ErrorMessage"] = "Некоторые поставщики не могут быть добавлены, так как поставщики с таким же ИНН уже есть в справочнике";
            }

            return RedirectToAction(nameof(Index));
        }

        public class CategoriesDataResult
        {
            public class DataItem
            {
                public Guid Id { get; set; }
                public string Name { get; set; }
            }

            public class CategoryItem
            {
                public Guid? Id { get; set; }
                public string Name { get; set; }
                public Guid? PrimaryContactId { get; set; }
                public Guid? SecondaryContactId { get; set; }
                public bool IsDefault { get; set; }
            }

            public List<DataItem> AvailableCategories { get; set; }
            public List<DataItem> ContactPersons { get; set; }
            public List<CategoryItem> Categories { get; internal set; }
        }

        string GetCatNameInHierarchy(List<NomenclatureCategoryVm> availableCategories, NomenclatureCategoryVm category)
        {
            if (category.ParentId.HasValue)
            {
                var categoryName = category.IsDeleted ? "[Удалена]" : category.Name;
                var parentCategory = availableCategories.First(q => q.Id == category.ParentId.Value);
                return $"{GetCatNameInHierarchy(availableCategories, parentCategory)} > {categoryName}";
            }

            return category.Name;
        }

        [HttpGet]
        public IActionResult CategoriesData(Guid supplierId)
        {
            var allCategories = _nomenclatureCategoryService.GetAll(true).ToList();
            
            var availableCategories = allCategories
                .Select(q => new CategoriesDataResult.DataItem { Id = q.Id, Name = GetCatNameInHierarchy(allCategories, q) })
                .ToList();

            var contactPersons = _supplierService.GetContactPersonsBySupplier(supplierId)
                .Select(q => new CategoriesDataResult.DataItem { Id = q.Id, Name = q.FullName })
                .ToList();

            var categories = _supplierService.GetSupplierNomenclatureCategories(supplierId)
                .Select(q => new CategoriesDataResult.CategoryItem
                {
                    Id = q.NomenclatureCategoryId,
                    Name = q.NomenclatureCategoryFullName,
                    PrimaryContactId = q.NomenclatureCategoryPrimaryContactId,
                    SecondaryContactId = q.NomenclatureCategorySecondaryContactId,
                    IsDefault = q.IsDefaultSupplierCategory
                })
                .ToList();

            var result = new CategoriesDataResult
            {
                AvailableCategories = availableCategories,
                ContactPersons = contactPersons,
                Categories = categories
            };

            return Json(result);
        }

        public class SaveCategoriesPostItem
        {
            public Guid? Id { get; set; }
            public string Name { get; set; }
            public Guid? PrimaryContactId { get; set; }
            public Guid? SecondaryContactId { get; set; }
            public bool IsDefault { get; set; }
        }

        [HttpPost]
        public IActionResult SaveCategories([FromQuery] Guid supplierId, [FromBody] IEnumerable<SaveCategoriesPostItem> categories)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _supplierService.RemoveSupplierNomenclatureCategories(supplierId);
                    _supplierService.SaveSupplierNomenclatureCategoryContacts(supplierId,
                        categories
                            .Where(nc => nc.Id.HasValue)
                            .Select(nc => (nc.Id.Value, nc.PrimaryContactId, nc.SecondaryContactId, nc.IsDefault)));

                    transaction.Commit();

                    return CategoriesData(supplierId);
                }
                catch (Exception)
                {
                    return BadRequest();
                }
            }
        }
    }
}

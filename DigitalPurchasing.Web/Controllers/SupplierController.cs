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
        private const string SameInnErrorMessage = "Контрагент с таким ИНН уже есть в системе";

        public SupplierController(
            ISupplierService supplierService,
            INomenclatureCategoryService nomenclatureCategoryService)
        {
            _supplierService = supplierService;
            _nomenclatureCategoryService = nomenclatureCategoryService;
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
        public IActionResult Autocomplete([FromQuery] string q) => Json(_supplierService.Autocomplete(new AutocompleteOptions { Query = q }));

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

        private void LoadRelatedData(SupplierEditVm vm, Guid supplierId)
        {
            vm.ContactPersons = _supplierService.GetContactPersonsBySupplier(supplierId);
            vm.NomenclatureCategoies = _supplierService.GetSupplierNomenclatureCategories(supplierId);
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

        [HttpPost]
        public IActionResult SaveSupplierCategories(SupplierEditVm vm, Guid supplierId)
        {
            if (ModelState.IsValid)
            {
                var defaultCategory = vm.NomenclatureCategoies.FirstOrDefault(_ => _.IsDefaultSupplierCategory);
                if (defaultCategory != null)
                {
                    var supplier = _supplierService.GetById(supplierId);
                    if (supplier.CategoryId != defaultCategory.NomenclatureCategoryId)
                    {
                        if (supplier.CategoryId.HasValue)
                        {
                            _supplierService.RemoveSupplierNomenclatureCategoryContacts(supplierId, supplier.CategoryId.Value);
                        }
                        supplier.CategoryId = defaultCategory.NomenclatureCategoryId;
                        _supplierService.Update(supplier);
                    }
                }

                _supplierService.SaveSupplierNomenclatureCategoryContacts(
                    supplierId,
                    vm.NomenclatureCategoies.Select(nc => (nc.NomenclatureCategoryId, nc.NomenclatureCategoryPrimaryContactId, nc.NomenclatureCategorySecondaryContactId)));
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Edit), new { id = supplierId });
        }

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
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(SupplierEditVm vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _supplierService.CreateSupplier(vm.Supplier.Adapt<SupplierVm>(), User.CompanyId());
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
                        category = _nomenclatureCategoryService.CreateOrUpdate(item.MainCategory, null);
                        if (!string.IsNullOrWhiteSpace(item.SubCategory1))
                        {
                            category = _nomenclatureCategoryService.CreateOrUpdate(item.SubCategory1, category.Id);
                            if (!string.IsNullOrWhiteSpace(item.SubCategory2))
                            {
                                category = _nomenclatureCategoryService.CreateOrUpdate(item.SubCategory2, category.Id);
                            }
                        }
                    }

                    Guid supplierId = _supplierService.CreateSupplier(new SupplierVm()
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

                    Guid? mainContactId = item.ContactSpecified
                        ? (Guid?)_supplierService.AddContactPerson(new SupplierContactPersonVm()
                        {
                            SupplierId = supplierId,
                            Email = item.ContactEmail,
                            FirstName = item.ContactFirstName,
                            LastName = item.ContactLastName,
                            JobTitle = item.ContactJobTitle,
                            MobilePhoneNumber = item.ContactMobilePhone,
                            PhoneNumber = item.ContactPhone
                        })
                    : null;

                    if (mainContactId.HasValue && category != null)
                    {
                        _supplierService.SaveSupplierNomenclatureCategoryContacts(supplierId,
                            new List<(Guid nomenclatureCategoryId, Guid? primarySupplierContactId, Guid? secondarySupplierContactId)>()
                            {
                                (category.Id, mainContactId.Value, null)
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
    }
}

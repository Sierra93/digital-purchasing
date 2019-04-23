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

namespace DigitalPurchasing.Web.Controllers
{
    public class SupplierController : Controller
    {
        public class DeleteContactPersonPost
        {
            [JsonProperty("id")] public Guid PersonId { get; set; }
        }

        private readonly ISupplierService _supplierService;
        private readonly IHtmlHelper _htmlHelper;

        public SupplierController(ISupplierService supplierService, IHtmlHelper htmlHelper)
        {
            _supplierService = supplierService;
            _htmlHelper = htmlHelper;
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
                catch (SameInnException e)
                {
                    ModelState.AddModelError(string.Empty, "Поставщик с таким же ИНН уже зарегистрирован в системе");
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
    }
}

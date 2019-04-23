using System;
using System.Collections.Generic;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.Supplier;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DigitalPurchasing.Web.Controllers
{
    public class SupplierController : Controller
    {
        public class DeleteContactPersonPost
        {
            [JsonProperty("id")] public Guid PersonId { get; set; }
        }

        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService) => _supplierService = supplierService;

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
            var vm = _supplierService.GetById(id);
            if (vm == null) return NotFound();

            var contactPersons = _supplierService.GetContactPersonsBySupplier(id);

            return View(new SupplierEditVm
            {
                Supplier = vm.Adapt<SupplierEditVm.SupplierVm>(),
                ContactPersons = contactPersons
            });
        }

        [HttpPost]
        public IActionResult Edit(SupplierEditVm vm)
        {
            if (ModelState.IsValid)
            {
                _supplierService.Update(vm.Supplier.Adapt<SupplierVm>());
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
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

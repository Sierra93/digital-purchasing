using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.Customer;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DigitalPurchasing.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService) => _customerService = customerService;

        [HttpGet]
        public IActionResult Autocomplete([FromQuery] string q) => Json(_customerService.Autocomplete(new AutocompleteOptions { Query = q }));

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Data(VueTableRequest request)
        {
            var result = _customerService.GetData(
                request.Page,
                request.PerPage,
                request.SortField,
                request.SortAsc,
                request.Search);

            var nextUrl = Url.Action(nameof(Data), "Customer", request.NextPageRequest(), Request.Scheme);
            var prevUrl = Url.Action(nameof(Data), "Customer", request.PrevPageRequest(), Request.Scheme);

            var dataItems = result.Data.Adapt<List<CustomerIndexDataItemEdit>>();
            foreach (var dataItem in dataItems)
            {
                dataItem.EditUrl = Url.Action(nameof(Edit), new { id = dataItem.Id });
            }

            return Json(new VueTableResponse<CustomerIndexDataItemEdit, VueTableRequest>(dataItems, request, result.Total, nextUrl, prevUrl));
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var data = _customerService.GetById(id);
            if (data == null) return NotFound();

            var vm = data.Adapt<CustomerEditVm>();

            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(CustomerEditVm vm)
        {
            if (ModelState.IsValid)
            {
                _customerService.Update(vm.Adapt<CustomerVm>());
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        public IActionResult Create()
        {
            var vm = new CustomerEditVm();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(CustomerEditVm vm)
        {
            if (ModelState.IsValid)
            {
                _customerService.CreateCustomer(vm.Name);
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult Delete([FromBody]DeleteVm vm)
        {
            try
            {
                _customerService.Delete(vm.Id);
            }
            catch (Services.Exceptions.CustomerInUseException)
            {
                return BadRequest(new { reason = "CUSTOMER_IN_USE" });
            }

            return Ok();
        }
    }
}

using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.Core;
using DigitalPurchasing.Web.ViewModels.Customer;
using Mapster;
using Microsoft.AspNetCore.Mvc;
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
                //dataItem.EditUrl = Url.Action(nameof(Edit), new { id = dataItem.Id });
            }

            return Json(new VueTableResponse<CustomerIndexDataItemEdit, VueTableRequest>(dataItems, request, result.Total, nextUrl, prevUrl));
        }
    }
}

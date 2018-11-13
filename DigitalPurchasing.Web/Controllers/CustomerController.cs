using DigitalPurchasing.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService) => _customerService = customerService;

        [HttpGet]
        public IActionResult Autocomplete([FromQuery] string q) => Json(_customerService.Autocomplete(new AutocompleteOptions { Query = q }));
    }
}

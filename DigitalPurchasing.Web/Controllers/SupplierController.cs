using DigitalPurchasing.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService) => _supplierService = supplierService;

        [HttpGet]
        public IActionResult Autocomplete([FromQuery] string q) => Json(_supplierService.Autocomplete(new AutocompleteOptions { Query = q }));
    }
}

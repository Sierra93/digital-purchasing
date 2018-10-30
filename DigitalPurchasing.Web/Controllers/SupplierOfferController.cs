using System;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.ViewModels.SupplierOffer;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class SupplierOfferController : Controller
    {
        private readonly ISupplierOfferService _supplierOfferService;

        public SupplierOfferController(ISupplierOfferService supplierOfferService) => _supplierOfferService = supplierOfferService;

        public IActionResult Edit(Guid id)
        {
            var vm = _supplierOfferService.GetById(id);

            if (vm.Status == SupplierOfferStatus.MatchColumns)
            {
                return View("EditMatchColumns", vm);
            }

            return NotFound();
        }

        public IActionResult ColumnsData(Guid id)
        {
            var data = _supplierOfferService.GetColumnsData(id);
            return Ok(data);
        }

        [HttpPost]
        public IActionResult SaveColumns([FromBody] SupplierOfferSaveColumnsVm model)
        {
            _supplierOfferService.SaveColumns(model.SupplierOfferId, model);
            return Ok();
        }
    }
}

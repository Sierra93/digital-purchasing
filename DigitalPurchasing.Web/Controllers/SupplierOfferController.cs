using System;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.ViewModels.SupplierOffer;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class SupplierOfferController : Controller
    {
        public class UpdateSupplierNameVm
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        private readonly ISupplierOfferService _supplierOfferService;

        public SupplierOfferController(ISupplierOfferService supplierOfferService) => _supplierOfferService = supplierOfferService;

        public IActionResult Edit(Guid id)
        {
            var vm = _supplierOfferService.GetById(id);

            if (vm.Status == SupplierOfferStatus.MatchColumns)
            {
                return View("EditMatchColumns", vm);
            }

            if (vm.Status == SupplierOfferStatus.MatchItems)
            {
                return View("EditMatchItems", vm);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult UpdateSupplierName([FromBody] UpdateSupplierNameVm model)
        {
            _supplierOfferService.UpdateSupplierName(model.Id, model.Name);
            return Ok();
        }

        #region # MatchColumns

        public IActionResult ColumnsData(Guid id)
        {
            var data = _supplierOfferService.GetColumnsData(id);
            return Ok(data);
        }

        [HttpPost]
        public IActionResult SaveColumns([FromBody] SupplierOfferSaveColumnsVm model)
        {
            _supplierOfferService.SaveColumns(model.SupplierOfferId, model);
            _supplierOfferService.GenerateRawItems(model.SupplierOfferId);
            _supplierOfferService.UpdateStatus(model.SupplierOfferId, SupplierOfferStatus.MatchItems);
            return Ok();
        }

        #endregion

        #region # MatchItems

        [HttpGet]
        public IActionResult MatchItemsData(Guid id)
        {
            var response = _supplierOfferService.MatchItemsData(id);
            return Json(response);
        }

        //[HttpPost]
        //public IActionResult SaveMatchItem([FromBody] SaveMatchItemVm model)
        //{
        //    var nomenclature = _nomenclatureService.AutocompleteSingle(model.NomenclatureId);
        //    _uomService.SaveConversionRate(model.UomId, nomenclature.Data.BatchUomId, nomenclature.Data.Id, model.FactorC, model.FactorN);
        //    _purchasingRequestService.SaveMatch(model.ItemId, model.NomenclatureId, model.UomId, model.FactorC, model.FactorN);
        //    _nomenclatureService.AddAlternative(model.NomenclatureId, model.ItemId);
        //    return Ok();
        //}

        #endregion
    }
}

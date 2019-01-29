using System;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.ViewModels.SupplierOffer;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class SupplierOfferController : Controller
    {
        public class UpdateSupplierNameData
        {
            public Guid Id { get; set; }
            public Guid? SupplierId { get; set; }
            public string Name { get; set; }
        }

        public class UpdateDeliveryCostData
        {
            public Guid Id { get; set; }
            public decimal DeliveryCost { get; set; }
        }

        public class SaveMatchItemVm
        {
            public Guid ItemId { get; set; }
            public Guid NomenclatureId { get; set; }
            public Guid UomId { get; set; }
            public decimal FactorN { get; set; }
            public decimal FactorC { get; set; }
        }

        private readonly ISupplierOfferService _supplierOfferService;
        private readonly INomenclatureService _nomenclatureService;
        private readonly IUomService _uomService;

        public SupplierOfferController(
            ISupplierOfferService supplierOfferService,
            INomenclatureService nomenclatureService,
            IUomService uomService)
        {
            _supplierOfferService = supplierOfferService;
            _nomenclatureService = nomenclatureService;
            _uomService = uomService;
        }

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
        public IActionResult UpdateSupplierName([FromBody] UpdateSupplierNameData model)
        {
            _supplierOfferService.UpdateSupplierName(model.Id, model.Name, model.SupplierId);
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateDeliveryCost([FromBody] UpdateDeliveryCostData model)
        {
            _supplierOfferService.UpdateDeliveryCost(model.Id, model.DeliveryCost);
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

        [HttpPost]
        public IActionResult SaveMatchItem([FromBody] SaveMatchItemVm model)
        {
            var nomenclature = _nomenclatureService.AutocompleteSingle(model.NomenclatureId);
            _uomService.SaveConversionRate(model.UomId, nomenclature.Data.BatchUomId, nomenclature.Data.Id, model.FactorC, model.FactorN);
            _supplierOfferService.SaveMatch(model.ItemId, model.NomenclatureId, model.UomId, model.FactorC, model.FactorN);
            _nomenclatureService.AddNomenclatureForSupplier(model.ItemId);
            return Ok();
        }

        #endregion

        #region Details

        public IActionResult Details(Guid id)
        {
            var vm = _supplierOfferService.GetDetailsById(id);
            return View(vm);
        }

        #endregion
    }
}

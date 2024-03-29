using System;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Web.ViewModels;
using DigitalPurchasing.Web.ViewModels.SupplierOffer;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public class SupplierOfferController : BaseController
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
            public Guid SupplierId { get; set; }
        }

        private readonly ISupplierOfferService _supplierOfferService;
        private readonly INomenclatureService _nomenclatureService;
        private readonly IUomService _uomService;
        private readonly IRootService _rootService;
        private readonly INomenclatureAlternativeService _nomenclatureAlternativeService;
        private readonly IPurchaseRequestService _purchaseRequestService;

        public SupplierOfferController(
            ISupplierOfferService supplierOfferService,
            INomenclatureService nomenclatureService,
            IUomService uomService,
            IRootService rootService,
            INomenclatureAlternativeService nomenclatureAlternativeService,
            IPurchaseRequestService purchaseRequestService)
        {
            _supplierOfferService = supplierOfferService;
            _nomenclatureService = nomenclatureService;
            _uomService = uomService;
            _rootService = rootService;
            _nomenclatureAlternativeService = nomenclatureAlternativeService;
            _purchaseRequestService = purchaseRequestService;
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

        public IActionResult NomenclatureMappingAutocomplete(Guid supplierOfferId, string q)
        {
            var so = _supplierOfferService.GetById(supplierOfferId);
            var pr = _purchaseRequestService.MatchItemsData(so.PurchaseRequestId);
            var autocompleteResult = _nomenclatureService.Autocomplete(new AutocompleteOptions { Query = q, OwnerId = User.CompanyId() });
            var noms2InPr = from item in autocompleteResult.Items
                            select new
                            {
                                item,
                                inPr = pr.Items.Any(_ => _.NomenclatureId == item.Id)
                            };
            autocompleteResult.Items = noms2InPr.OrderByDescending(_ => _.inPr).Select(_ => _.item).ToList();
            return Json(autocompleteResult);
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

        [HttpPost]
        public IActionResult Delete([FromBody] DeleteVm model)
        {
            if (!User.CanDeleteSupplierOffers()) return BadRequest();

            var result = _supplierOfferService.Delete(model.Id);
            if (result.IsSuccess) return Ok();

            return BadRequest();
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
        public async Task<IActionResult> SaveMatchItem([FromBody] SaveMatchItemVm model)
        {
            var companyId = User.CompanyId();
            var fromUomId = model.UomId;
            var nomenclature = _nomenclatureService.GetById(model.NomenclatureId);
            var nomenclatureAlternativeId = _nomenclatureAlternativeService.AddNomenclatureForSupplier(model.ItemId); // must be above than "SaveConversionRate"

            var isSaved = false;

            if (nomenclatureAlternativeId.HasValue && model.FactorN > 0)
            {
                if (fromUomId == nomenclature.MassUomId)
                {
                    // todo: get !1! from uom
                    var mass = 1 / model.FactorN;
                    await _nomenclatureAlternativeService.UpdateMassUom(nomenclatureAlternativeId.Value, fromUomId, mass);
                    isSaved = true;
                }
                else if (fromUomId == await _uomService.GetPackagingUomId(companyId))
                {
                    var quantityInPackage = model.FactorN;
                    await _nomenclatureAlternativeService.UpdatePackUom(nomenclatureAlternativeId.Value, nomenclature.BatchUomId, quantityInPackage);
                    isSaved = true;
                }
            }

            if (!isSaved)
            {
                _uomService.SaveConversionRate(
                    companyId,
                    fromUomId,
                    nomenclature.BatchUomId,
                    nomenclatureAlternativeId,
                    model.FactorC,
                    model.FactorN);
            }

            _supplierOfferService.SaveMatch(model.ItemId, model.NomenclatureId, model.UomId, model.FactorC, model.FactorN);
            
            var clId = await _supplierOfferService.GetCLIdBySoItem(model.ItemId);

            var rootId = await _rootService.GetIdByCL(clId);
            var isAllMatched = _supplierOfferService.IsAllMatchedBySoItem(model.ItemId);
            await _rootService.SetStatus(rootId, isAllMatched
                ? RootStatus.EverythingMatches
                : RootStatus.MatchingRequired);
            
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

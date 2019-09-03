using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public partial class PurchaseRequestController
    {
        public class SaveMatchItemVm
        {
            public Guid ItemId { get; set; }
            public Guid NomenclatureId { get; set; }
            public Guid UomId { get; set; }
            public decimal FactorN { get; set; }
            public decimal FactorC { get; set; }
            public Guid CustomerId { get; set; }
        }

        [HttpGet]
        public IActionResult MatchItemsData(Guid id)
        {
            var response = _purchasingRequestService.MatchItemsData(id);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMatchItem([FromBody] SaveMatchItemVm model)
        {
            var companyId = User.CompanyId();
            var fromUomId = model.UomId;
            var nomenclature = _nomenclatureService.GetById(model.NomenclatureId);
            var nomenclatureAlternativeId =
                _nomenclatureAlternativeService.AddNomenclatureForCustomer(model.ItemId);

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
                else if (fromUomId == await _uomService.GetPackagingUom(companyId))
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
                    model.UomId,
                    nomenclature.BatchUomId,
                    nomenclatureAlternativeId,
                    model.FactorC,
                    model.FactorN);
            }

            _purchasingRequestService.SaveMatch(model.ItemId, model.NomenclatureId, model.UomId, model.FactorC, model.FactorN);

            return Ok();
        }
    }
}

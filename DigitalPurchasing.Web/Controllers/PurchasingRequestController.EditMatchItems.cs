using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Controllers
{
    public partial class PurchasingRequestController
    {
        [HttpGet]
        public IActionResult MatchItemsData(Guid id)
        {
            var response = _purchasingRequestService.MatchItemsData(id);
            return Json(response);
        }

        [HttpPost]
        public IActionResult SaveMatchItem([FromBody] SaveMatchItemVm model)
        {
            var nomenclature = _nomenclatureService.AutocompleteSingle(model.NomenclatureId);
            _uomService.SaveConversionRate(model.UomId, nomenclature.Data.BatchUomId, nomenclature.Data.Id, model.FactorC, model.FactorN);
            _purchasingRequestService.SaveMatch(model.ItemId, model.NomenclatureId, model.UomId, model.FactorC, model.FactorN);
            _nomenclatureService.AddAlternative(model.NomenclatureId, model.ItemId);
            return Ok();
        }
    }
}

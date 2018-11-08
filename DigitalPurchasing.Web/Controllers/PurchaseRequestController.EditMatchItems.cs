using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        }

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
            _nomenclatureService.AddAlternativeCustomer(model.NomenclatureId, model.ItemId);
            return Ok();
        }
    }
}

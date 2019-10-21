using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
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

        public class CreateAndSaveMatchItemPost
        {
            public Guid ItemId { get; set; }
            public string Name { get; set; }
            public Guid UomId { get; set; }
        }

        public class CreateAndSaveMatchItemResult
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }

            public Guid NomenclatureId { get; set; }

            public static CreateAndSaveMatchItemResult Success(Guid nomenclatureId) => new CreateAndSaveMatchItemResult { IsSuccess = true, NomenclatureId = nomenclatureId };
            public static CreateAndSaveMatchItemResult Error(string message) => new CreateAndSaveMatchItemResult { IsSuccess = false, Message = message };
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

        [HttpPost]
        public async Task<IActionResult> CreateAndSaveMatchItem([FromBody] CreateAndSaveMatchItemPost post)
        {
            var companyId = User.CompanyId();

            var massUomId = await _uomService.GetMassUom(companyId);
            if (massUomId == Guid.Empty)
            {
                return Ok(CreateAndSaveMatchItemResult.Error("Ошибка. Установите ЕИ по-умолчанию для массы"));
            }

            var resourceUomId = await _uomService.GetResourceUom(companyId);
            if (resourceUomId == Guid.Empty)
            {
                return Ok(CreateAndSaveMatchItemResult.Error("Ошибка. Установите ЕИ по-умолчанию  для ресурса"));
            }

            var resourceBatchUom = await _uomService.GetResourceBatchUom(companyId);
            if (resourceBatchUom == Guid.Empty)
            {
                return Ok(CreateAndSaveMatchItemResult.Error("Ошибка. Установите ЕИ по-умолчанию  для ЕИ ресурса"));
            }

            var category = _nomenclatureCategoryService.CreateOrUpdate(companyId, "---", null);

            var model = new NomenclatureVm
            {
                Name = post.Name,
                BatchUomId = post.UomId,
                MassUomId = massUomId,
                ResourceUomId = resourceUomId,
                ResourceBatchUomId = resourceBatchUom,
                CategoryId = category.Id,
            };

            var nomenclature = _nomenclatureService.CreateOrUpdate(model, companyId);

            _nomenclatureAlternativeService.AddNomenclatureForCustomer(post.ItemId);

            _purchasingRequestService.SaveMatch(post.ItemId, nomenclature.Id, post.UomId, 1, 0);
            
            return Ok(CreateAndSaveMatchItemResult.Success(nomenclature.Id));
        }
    }
}

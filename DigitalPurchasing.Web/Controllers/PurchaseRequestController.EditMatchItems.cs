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

        public class CreateAndSaveAllPost
        {
            public Guid Id { get; set; }
        }

        public class CreateAndSaveAllResult
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }

            public int Count { get; set; }

            public static CreateAndSaveAllResult Success(int count) => new CreateAndSaveAllResult { IsSuccess = true, Count = count };
            public static CreateAndSaveAllResult Error(string message) => new CreateAndSaveAllResult { IsSuccess = false, Message = message };
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
                    model.UomId,
                    nomenclature.BatchUomId,
                    nomenclatureAlternativeId,
                    model.FactorC,
                    model.FactorN);
            }

            _purchasingRequestService.SaveMatch(model.ItemId, model.NomenclatureId, model.UomId, model.FactorC, model.FactorN);

            return Ok();
        }

        public const string DefaultUomMassError = "Установите ЕИ по-умолчанию для массы";
        public const string DefaultUomResourceError = "Установите ЕИ по-умолчанию для ресурса";
        public const string DefaultUomResourceBatchError = "Установите ЕИ по-умолчанию для ЕИ ресурса";

        private NomenclatureCategoryVm GetDefaultCategory(Guid companyId)
            => _nomenclatureCategoryService.CreateOrUpdate(companyId, "---", null);

        [HttpPost]
        public async Task<IActionResult> CreateAndSaveMatchItem([FromBody] CreateAndSaveMatchItemPost post)
        {
            var companyId = User.CompanyId();

            var massUomId = await _uomService.GetMassUomId(companyId);
            if (massUomId == Guid.Empty)
            {
                return Ok(CreateAndSaveMatchItemResult.Error(DefaultUomMassError));
            }

            var resourceUomId = await _uomService.GetResourceUomId(companyId);
            if (resourceUomId == Guid.Empty)
            {
                return Ok(CreateAndSaveMatchItemResult.Error(DefaultUomResourceError));
            }

            var resourceBatchUom = await _uomService.GetResourceBatchUomId(companyId);
            if (resourceBatchUom == Guid.Empty)
            {
                return Ok(CreateAndSaveMatchItemResult.Error(DefaultUomResourceBatchError));
            }

            var category = GetDefaultCategory(companyId);

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
            _purchasingRequestService.SaveMatch(post.ItemId, nomenclature.Id, post.UomId, 1, 0);
            _nomenclatureAlternativeService.AddNomenclatureForCustomer(post.ItemId);
            
            return Ok(CreateAndSaveMatchItemResult.Success(nomenclature.Id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAndSaveAll([FromBody]CreateAndSaveAllPost post)
        {
            var companyId = User.CompanyId();

            var massUomId = await _uomService.GetMassUomId(companyId);
            if (massUomId == Guid.Empty)
            {
                return Ok(CreateAndSaveAllResult.Error(DefaultUomMassError));
            }

            var resourceUomId = await _uomService.GetResourceUomId(companyId);
            if (resourceUomId == Guid.Empty)
            {
                return Ok(CreateAndSaveAllResult.Error(DefaultUomResourceError));
            }

            var resourceBatchUom = await _uomService.GetResourceBatchUomId(companyId);
            if (resourceBatchUom == Guid.Empty)
            {
                return Ok(CreateAndSaveAllResult.Error(DefaultUomResourceBatchError));
            }

            var category = GetDefaultCategory(companyId);

            var itemsData = _purchasingRequestService.MatchItemsData(post.Id);

            var count = 0;

            foreach (var item in itemsData.Items.Where(q => q.RawUomMatchId.HasValue && !q.NomenclatureId.HasValue))
            {
                var uomId = item.RawUomMatchId.Value;

                var model = new NomenclatureVm
                {
                    Name = item.RawName,
                    BatchUomId = uomId,
                    MassUomId = massUomId,
                    ResourceUomId = resourceUomId,
                    ResourceBatchUomId = resourceBatchUom,
                    CategoryId = category.Id
                };

                var nomenclature = _nomenclatureService.CreateOrUpdate(model, companyId);
                _purchasingRequestService.SaveMatch(item.Id, nomenclature.Id, uomId, 1, 0);
                _nomenclatureAlternativeService.AddNomenclatureForCustomer(item.Id);

                count++;
            }

            return Ok(CreateAndSaveAllResult.Success(count));
        }
    }
}

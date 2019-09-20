using System;
using System.Linq;
using System.Threading.Tasks;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class ConversionRateService : IConversionRateService
    {
        private readonly ApplicationDbContext _db;
        private readonly IUomService _uomService;
        private readonly INomenclatureService _nomenclatureService;
        private readonly INomenclatureAlternativeService _nomenclatureAlternativeService;

        public ConversionRateService(
            ApplicationDbContext db,
            IUomService uomService,
            INomenclatureService nomenclatureService,
            INomenclatureAlternativeService nomenclatureAlternativeService)
        {
            _db = db;
            _uomService = uomService;
            _nomenclatureService = nomenclatureService;
            _nomenclatureAlternativeService = nomenclatureAlternativeService;
        }

        private async Task<UomConversionRateResponse> GetConversionRate(
            Guid fromUomId,
            NomenclatureVm nomenclature,
            NomenclatureAlternativeVm nomenclatureAlternative)
        {
            var result = new UomConversionRateResponse { CommonFactor = 0, NomenclatureFactor = 0 };

            var toUomId = nomenclature.BatchUomId;

            // kg -> ...
            var calcFromUom = fromUomId == nomenclature.MassUomId || fromUomId == nomenclatureAlternative?.MassUomId;
            if (calcFromUom)
            {
                // todo: get !1! from uom
                if (nomenclatureAlternative?.MassUomValue > 0)
                {
                    result.CommonFactor = 1 / nomenclatureAlternative.MassUomValue;
                }
                else if (nomenclature.MassUomValue > 0)
                {
                    result.CommonFactor = 1 / nomenclature.MassUomValue;
                }
            }

            // packaging -> ...
            var packagingUom = await _uomService.GetPackagingUom(nomenclature.OwnerId);

            var calcFromPack = fromUomId == packagingUom;
            if (calcFromPack)
            {
                if (nomenclatureAlternative?.PackUomId != null
                    && nomenclatureAlternative.PackUomValue.HasValue
                    && nomenclatureAlternative.PackUomValue.Value > 0)
                {
                    var packUom = _uomService.GetById(nomenclatureAlternative.PackUomId.Value);
                    var toUom = _uomService.GetById(toUomId);
                    var isSameUom = packUom.Id == toUom.Id;
                    if (isSameUom)
                    {
                        result.CommonFactor = nomenclatureAlternative.PackUomValue.Value;
                    }
                    if (toUom.Quantity.HasValue && packUom.Quantity.HasValue)
                    {
                        result.CommonFactor = (nomenclatureAlternative.PackUomValue.Value * packUom.Quantity.Value) / toUom.Quantity.Value;
                    }
                } else if (nomenclature.PackUomId.HasValue && nomenclature.PackUomValue > 0)
                {
                    var packUom = _uomService.GetById(nomenclature.PackUomId.Value);
                    var toUom = _uomService.GetById(toUomId);

                    var isSameUom = packUom.Id == toUom.Id;

                    if (isSameUom)
                    {
                        result.CommonFactor = nomenclature.PackUomValue;
                    }
                    else if (toUom.Quantity.HasValue && packUom.Quantity.HasValue)
                    {
                        result.CommonFactor = (nomenclature.PackUomValue * packUom.Quantity.Value) / toUom.Quantity.Value;
                    }
                }
            }

            var conversionRates = _db.UomConversionRates
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(q => q.OwnerId == nomenclature.OwnerId)
                .Where(q => (q.FromUomId == fromUomId && q.ToUomId == toUomId) ||
                            (q.FromUomId == toUomId && q.ToUomId == fromUomId))
                .ToList();

            if (!conversionRates.Any())
            {
                return result;
            }

            if (result.CommonFactor == 0) /* don't overwrite conversion rate from mass */
            {
                var commonConversionRate = conversionRates.FirstOrDefault(q => q.NomenclatureId == null && q.NomenclatureAlternativeId == null);
                if (commonConversionRate != null)
                {
                    result.CommonFactor = commonConversionRate.FromUomId == fromUomId
                        ? commonConversionRate.Factor
                        : 1m / commonConversionRate.Factor;
                }
            }

            if (nomenclatureAlternative != null)
            {
                var nomenclatureConversionRate = conversionRates.FirstOrDefault(q => q.NomenclatureAlternativeId == nomenclatureAlternative.Id);
                if (nomenclatureConversionRate != null)
                {
                    result.NomenclatureFactor = nomenclatureConversionRate.FromUomId == fromUomId
                        ? nomenclatureConversionRate.Factor
                        : 1m / nomenclatureConversionRate.Factor;
                }
            }

            if (result.NomenclatureFactor == 0) /* don't overwrite conversion rate from alt nomenclature */
            {
                var nomenclatureConversionRate = conversionRates.FirstOrDefault(q => q.NomenclatureId == nomenclature.Id);
                if (nomenclatureConversionRate != null)
                {
                    result.NomenclatureFactor = nomenclatureConversionRate.FromUomId == fromUomId
                        ? nomenclatureConversionRate.Factor
                        : 1m / nomenclatureConversionRate.Factor;
                }
            }

            return result;
        }

        public async Task<UomConversionRateResponse> GetRate(Guid fromUomId, Guid nomenclatureId, Guid? customerId, Guid? supplierId)
        {
            var nomenclature = _nomenclatureService.GetById(nomenclatureId, true);

            NomenclatureAlternativeVm nomenclatureAlternative = null;

            if (customerId.HasValue)
            {
                nomenclatureAlternative = _nomenclatureAlternativeService.GetForCustomer(customerId.Value, nomenclatureId);
            }

            if (supplierId.HasValue)
            {
                nomenclatureAlternative = _nomenclatureAlternativeService.GetForSupplier(supplierId.Value, nomenclatureId);
            }

            var toUomId = nomenclature.BatchUomId;
            if (fromUomId == toUomId)
            {
                return new UomConversionRateResponse { CommonFactor = 1, NomenclatureFactor = 0 };
            }

            return await GetConversionRate(fromUomId, nomenclature, nomenclatureAlternative);
        }
    }
}

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

        public ConversionRateService(
            ApplicationDbContext db,
            IUomService uomService,
            INomenclatureService nomenclatureService)
        {
            _db = db;
            _uomService = uomService;
            _nomenclatureService = nomenclatureService;
        }

        private async Task<UomConversionRateResponse> GetConversionRate(Guid fromUomId, NomenclatureVm nomenclature)
        {
            var result = new UomConversionRateResponse { CommonFactor = 0, NomenclatureFactor = 0 };

            var toUomId = nomenclature.BatchUomId;

            // kg -> ...
            var calcFromUom = fromUomId == nomenclature.MassUomId && nomenclature.MassUomValue > 0;
            if (calcFromUom)
            {
                result.CommonFactor = 1 / nomenclature.MassUomValue;
            }

            // packaging -> ...
            var calcFromPack = fromUomId == await _uomService.GetPackagingUom(nomenclature.OwnerId)
                               && nomenclature.PackUomId.HasValue
                               && nomenclature.PackUomValue > 0;
            if (calcFromPack)
            {
                var packUom = _uomService.GetById(nomenclature.PackUomId.Value);
                var toUom = _uomService.GetById(toUomId);
                if (toUom.Quantity.HasValue && packUom.Quantity.HasValue)
                {
                    result.CommonFactor = toUom.Quantity.Value / (nomenclature.PackUomValue * packUom.Quantity.Value);
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
                var commonConversionRate = conversionRates.FirstOrDefault(q => q.NomenclatureId == null);
                
                if (commonConversionRate != null)
                {
                    result.CommonFactor = commonConversionRate.FromUomId == fromUomId
                        ? commonConversionRate.Factor
                        : 1m / commonConversionRate.Factor;
                }
            }

            var nomenclatureConversionRate = conversionRates.FirstOrDefault(q => q.NomenclatureId == nomenclature.Id);
            if (nomenclatureConversionRate != null)
            {
                result.NomenclatureFactor = nomenclatureConversionRate.FromUomId == fromUomId
                    ? nomenclatureConversionRate.Factor
                    : 1m / nomenclatureConversionRate.Factor;
            }

            return result;
        }

        public async Task<UomConversionRateResponse> GetRate(Guid fromUomId, Guid nomenclatureId)
        {
            var nomenclature = _nomenclatureService.GetById(nomenclatureId, true);

            var toUomId = nomenclature.BatchUomId;
            if (fromUomId == toUomId)
            {
                return new UomConversionRateResponse { CommonFactor = 1, NomenclatureFactor = 0 };
            }

            return await GetConversionRate(fromUomId, nomenclature);
        }
    }
}

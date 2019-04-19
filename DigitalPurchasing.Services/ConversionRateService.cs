using System;
using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class ConversionRateService : IConversionRateService
    {
        private readonly ApplicationDbContext _db;
        private readonly IUomService _uomService;

        public ConversionRateService(
            ApplicationDbContext db,
            IUomService uomService)
        {
            _db = db;
            _uomService = uomService;
        }

        private UomConversionRateResponse GetConversionRate(
            Guid ownerId,
            Guid fromUomId,
            Guid toUomId,
            Guid nomenclatureId,
            Guid nomenclatureMassUomId,
            decimal nomenclatureMassValue) // todo: pass nomenclature dto
        {
            var result = new UomConversionRateResponse { CommonFactor = 0, NomenclatureFactor = 0 };

            var calcFromUom = fromUomId == nomenclatureMassUomId && nomenclatureMassValue > 0;
            if (calcFromUom)
            {
                var toUom = _uomService.GetById(toUomId);
                if (toUom.Quantity.HasValue)
                {
                    result.CommonFactor = nomenclatureMassValue / toUom.Quantity.Value;
                }
            }

            var conversionRates = _db.UomConversionRates
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(q => q.OwnerId == ownerId)
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

            var nomenclatureConversionRate = conversionRates.FirstOrDefault(q => q.NomenclatureId == nomenclatureId);
            if (nomenclatureConversionRate != null)
            {
                result.NomenclatureFactor = nomenclatureConversionRate.FromUomId == fromUomId
                    ? nomenclatureConversionRate.Factor
                    : 1m / nomenclatureConversionRate.Factor;
            }

            return result;
        }

        public UomConversionRateResponse GetRate(Guid fromUomId, Guid nomenclatureId)
        {
            // todo: use NomenclatureService ?
            var qryNomenclatures = _db.Nomenclatures.AsNoTracking().IgnoreQueryFilters().AsQueryable();
            var nomenclature = qryNomenclatures.First(q => q.Id == nomenclatureId);

            var toUomId = nomenclature.BatchUomId;
            if (fromUomId == toUomId)
            {
                return new UomConversionRateResponse { CommonFactor = 1, NomenclatureFactor = 0 };
            }

            return GetConversionRate(
                nomenclature.OwnerId,
                fromUomId, nomenclature.BatchUomId,
                nomenclatureId, nomenclature.MassUomId, nomenclature.MassUomValue);
        }
    }
}

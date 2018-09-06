using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class UomService : IUomService
    {
        private readonly ApplicationDbContext _db;

        public UomService(ApplicationDbContext db) => _db = db;

        public UomDataResult GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }

            var qry =  _db.UnitsOfMeasurements.AsNoTracking();
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults.Skip((page-1)*perPage).Take(perPage).ProjectToType<UomResult>().ToList();
            return new UomDataResult
            {
                Total = total,
                Data = result
            };
        }

        public UomResult CreateUom(string name)
        {
            var entry = _db.UnitsOfMeasurements.Add(new UnitsOfMeasurement {Name = name});
            _db.SaveChanges();
            return entry.Entity.Adapt<UomResult>();
        }

        public IEnumerable<UomResult> GetAll() => _db.UnitsOfMeasurements.ProjectToType<UomResult>().ToList();

        public UomConversionRateResult GetConversionRate(Guid fromUomId, Guid toUomId, Guid nomenclatureId)
        {
            var conversionRates = _db.UomConversionRates.AsNoTracking()
                .Where(q =>
                    ( q.FromUomId == fromUomId && q.ToUomId == toUomId ) ||
                    ( q.FromUomId == toUomId && q.ToUomId == fromUomId ));

            if (!conversionRates.Any())
            {
                return null;
            }

            var result = new UomConversionRateResult();
            var commonConversionRate = conversionRates.FirstOrDefault(q => q.NomenclatureId == null);
            var nomenclatureConversionRate = conversionRates.FirstOrDefault(q => q.NomenclatureId == nomenclatureId);

            if (commonConversionRate != null)
            {
                result.CommonFactor = commonConversionRate.FromUomId == fromUomId
                    ? commonConversionRate.Factor
                    : 1m / commonConversionRate.Factor;
            }

            if (nomenclatureConversionRate != null)
            {
                result.NomenclatureFactor = nomenclatureConversionRate.FromUomId == fromUomId
                    ? nomenclatureConversionRate.Factor
                    : 1m / nomenclatureConversionRate.Factor;
            }

            return result;
        }
    }

    public class UomResultMapsterRegister : IRegister
    {
        public void Register(TypeAdapterConfig config) => config.NewConfig<UnitsOfMeasurement, UomResult>().Map(d => d.IsSystem, s => !s.OwnerId.HasValue);
    }
}

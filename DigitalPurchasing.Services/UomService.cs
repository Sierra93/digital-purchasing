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

        public UomIndexData GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }

            var qry =  _db.UnitsOfMeasurements.Where(q => !q.IsDeleted).AsNoTracking();
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults.Skip((page-1)*perPage).Take(perPage).ProjectToType<UomIndexDataItem>().ToList();
            return new UomIndexData
            {
                Total = total,
                Data = result
            };
        }

        public UomFactorData GetFactorData(Guid uomId, int page, int perPage, string sortField, bool sortAsc)
        {
            var qry = _db.UomConversionRates
                .Include(q => q.Nomenclature)
                .Include(q => q.ToUom)
                .Include(q => q.FromUom)
                .Where(q => q.ToUomId == uomId || q.FromUomId == uomId);

            var total = qry.Count();
            var orderedResults = qry.OrderBy(q => q.Id);
            var entities = orderedResults.Skip((page-1)*perPage).Take(perPage).ToList();
            var result = entities.Select(q => new UomFactorDataItem
            {
                Id = q.Id,
                Factor = q.FromUomId == uomId ? q.Factor : 1m / q.Factor,
                Nomenclature = q.Nomenclature?.Name,
                Uom = q.FromUomId == uomId ? q.ToUom.Name : q.FromUom.Name
            }).ToList();
            return new UomFactorData
            {
                Total = total,
                Data = result
            };
        }

        public UomResult CreateOrUpdate(string name)
        {
            var entity =
                _db.UnitsOfMeasurements.FirstOrDefault(q =>
                    q.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (entity == null)
            {
                var entry = _db.UnitsOfMeasurements.Add(new UnitsOfMeasurement {Name = name});
                _db.SaveChanges();
                entity = entry.Entity;
            }

            return entity.Adapt<UomResult>();
        }

        public IEnumerable<UomResult> GetAll() => _db.UnitsOfMeasurements.Where(q => !q.IsDeleted).ProjectToType<UomResult>().ToList();

        public UomConversionRateResponse GetConversionRate(Guid fromUomId, Guid nomenclatureId)
        {
            var toUomId = _db.Nomenclatures.First(q => q.Id == nomenclatureId).BatchUomId;
            var conversionRates = _db.UomConversionRates.AsNoTracking()
                .Where(q =>
                    ( q.FromUomId == fromUomId && q.ToUomId == toUomId ) ||
                    ( q.FromUomId == toUomId && q.ToUomId == fromUomId ));

            var result = new UomConversionRateResponse { CommonFactor = 0, NomenclatureFactor = 0 };

            if (!conversionRates.Any())
            {
                return result;
            }
            
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

        public UomAutocompleteResponse Autocomplete(string s)
        {
            var autocompleteItems = _db.UnitsOfMeasurements
                .AsNoTracking()
                .Where(q => q.Name.Contains(s) && !q.IsDeleted)
                .ProjectToType<UomAutocompleteResponse.AutocompleteItem>()
                .ToList();

            return new UomAutocompleteResponse { Items = autocompleteItems };
        }

        public BaseResult<UomAutocompleteResponse.AutocompleteItem> AutocompleteSingle(Guid id)
        {
            var entity = _db.UnitsOfMeasurements.Find(id);
            var data = entity?.Adapt<UomAutocompleteResponse.AutocompleteItem>();
            return new BaseResult<UomAutocompleteResponse.AutocompleteItem>(data);
        }

        public void SaveConversionRate(Guid fromUomId, Guid toUomId, Guid nomenclatureId, decimal factorC, decimal factorN)
        {
            var rateQry = _db.UomConversionRates.Where(q =>
                (q.FromUomId == fromUomId && q.ToUomId == toUomId) || (q.FromUomId == toUomId && q.ToUomId == fromUomId));

            var rate = factorC > 0 ? rateQry.FirstOrDefault() : rateQry.FirstOrDefault(q => q.NomenclatureId == nomenclatureId);
            if (rate == null)
            {
                var newRate = new UomConversionRate { FromUomId = fromUomId, ToUomId = toUomId };
                if (factorC > 0)
                {
                    newRate.Factor = factorC;
                }
                else
                {
                    newRate.Factor = factorN;
                    newRate.NomenclatureId = nomenclatureId;
                }
                _db.UomConversionRates.Add(newRate);
            }
            else
            {
                rate.Factor = factorC;
            }
            _db.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var entity = _db.UnitsOfMeasurements.Find(id);
            if (entity == null) return;
            entity.IsDeleted = true;
            _db.SaveChanges();
        }

        public UomVm GetById(Guid id) => _db.UnitsOfMeasurements.Find(id)?.Adapt<UomVm>();

        public UomVm Update(Guid id, string name)
        {
            var entity = _db.UnitsOfMeasurements.Find(id);
            entity.Name = name.Trim();
            _db.SaveChanges();
            return entity.Adapt<UomVm>();
        }
    }
}

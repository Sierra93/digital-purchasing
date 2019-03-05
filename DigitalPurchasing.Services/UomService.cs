using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DigitalPurchasing.Services
{
    public class UomService : IUomService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;

        public UomService(ApplicationDbContext db, IMemoryCache cache, ILogger<UomService> logger)
        {
            _db = db;
            _cache = cache;
            _logger = logger;
        }

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
                Uom = q.FromUomId == uomId ? q.ToUom.Name : q.FromUom.Name,
                FromId = q.FromUomId == uomId ? q.FromUomId : q.ToUomId,
                ToId = q.FromUomId == uomId ? q.ToUomId : q.FromUomId,
                NomenclatureId = q.NomenclatureId
            }).ToList();

            return new UomFactorData
            {
                Total = total,
                Data = result
            };
        }

        public UomResult CreateOrUpdate(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var entity =
                _db.UnitsOfMeasurements.FirstOrDefault(q =>
                    q.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (entity == null)
            {
                var normalizedName = name.CustomNormalize();
                var entry = _db.UnitsOfMeasurements.Add(new UnitsOfMeasurement { Name = name, NormalizedName = normalizedName });
                _db.SaveChanges();
                entity = entry.Entity;
            }

            return entity.Adapt<UomResult>();
        }

        public IEnumerable<UomResult> GetAll() => _db.UnitsOfMeasurements.Where(q => !q.IsDeleted).ProjectToType<UomResult>().ToList();

        public UomConversionRateResponse GetConversionRate(Guid ownerId, Guid fromUomId, Guid toUomId, Guid nomenclatureId)
        {
            var conversionRates = _db.UomConversionRates
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(q => q.OwnerId == ownerId)
                .Where(q => ( q.FromUomId == fromUomId && q.ToUomId == toUomId ) ||
                            ( q.FromUomId == toUomId && q.ToUomId == fromUomId ))
                .ToList();

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

        public UomConversionRateResponse GetConversionRate(Guid fromUomId, Guid nomenclatureId)
        {
            var qryNomenclatures = _db.Nomenclatures.AsNoTracking().IgnoreQueryFilters().AsQueryable();
            var nomenclature = qryNomenclatures.First(q => q.Id == nomenclatureId);
            
            var toUomId = nomenclature.BatchUomId;
            if (fromUomId == toUomId)
            {
                return new UomConversionRateResponse { CommonFactor = 1, NomenclatureFactor = 0 };
            }

            return GetConversionRate(nomenclature.OwnerId, fromUomId, nomenclature.BatchUomId, nomenclatureId);
        }

        public UomAutocompleteResponse Autocomplete(string s, Guid ownerId)
        {
            var response = new UomAutocompleteResponse
            {
                Items = new List<UomAutocompleteResponse.AutocompleteItem>()
            };

            if (string.IsNullOrEmpty(s)) return response;

            var cacheKey = Consts.CacheKeys.UomAutocomplete(ownerId, s);

            if (!_cache.TryGetValue(cacheKey, out List<UomAutocompleteResponse.AutocompleteItem> items)
            )
            {
                var normalizedName = s.CustomNormalize();

                var qry = _db.UnitsOfMeasurements
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .Where(q => q.OwnerId == ownerId);

                var autocompleteItems = qry
                    .Where(q => (q.Name == s || q.NormalizedName == normalizedName || q.Name.Contains(s)) && !q.IsDeleted)
                    .Select(q => new { q.Id, q.Name, q.NormalizedName, IsFullMatch = q.Name == s || q.NormalizedName == s})
                    .OrderByDescending(q => q.IsFullMatch)
                    .ToList();

                items = autocompleteItems.Adapt<List<UomAutocompleteResponse.AutocompleteItem>>();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(5));

                _cache.Set(cacheKey, items, cacheEntryOptions);
            }
            else
            {
                _logger.LogInformation($"Autocomplete cache hit - {cacheKey}");
            }

            response.Items = items;

            return response;
        }

        public BaseResult<UomAutocompleteResponse.AutocompleteItem> AutocompleteSingle(Guid id)
        {
            var entity = _db.UnitsOfMeasurements.Find(id);
            var data = entity?.Adapt<UomAutocompleteResponse.AutocompleteItem>();
            return new BaseResult<UomAutocompleteResponse.AutocompleteItem>(data);
        }

        public void SaveConversionRate(Guid fromUomId, Guid toUomId, Guid? nomenclatureId, decimal factorC, decimal factorN)
        {
            if (fromUomId == toUomId) return; // don't store in database, factor = 1

            var rateQry = _db.UomConversionRates.Where(q =>
                (q.FromUomId == fromUomId && q.ToUomId == toUomId) || (q.FromUomId == toUomId && q.ToUomId == fromUomId));

            var isCommon = factorC > 0;

            var rate = isCommon ? rateQry.FirstOrDefault() : rateQry.FirstOrDefault(q => q.NomenclatureId == nomenclatureId);
            if (rate == null)
            {
                var newRate = new UomConversionRate { FromUomId = fromUomId, ToUomId = toUomId };
                if (isCommon)
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
                var factor = isCommon ? factorC : factorN;
                rate.Factor = rate.FromUomId == fromUomId ? factor : 1m/factor;
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
            entity.NormalizedName = name.CustomNormalize();
            _db.SaveChanges();
            return entity.Adapt<UomVm>();
        }

        public void DeleteConversionRate(Guid id)
        {
            _db.Remove(_db.UomConversionRates.Find(id));
            _db.SaveChanges();
        }
    }
}

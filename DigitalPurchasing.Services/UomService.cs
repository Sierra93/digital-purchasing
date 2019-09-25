using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using EFCore.BulkExtensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MoreLinq;

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
                .Include(q => q.NomenclatureAlternative.Link.Customer)
                .Include(q => q.NomenclatureAlternative.Link.Supplier)
                .Include(q => q.Nomenclature)
                .Include(q => q.ToUom)
                .Include(q => q.FromUom)
                .Where(q => q.ToUomId == uomId || q.FromUomId == uomId);

            var total = qry.Count();
            var orderedResults = qry.OrderBy(q => q.Id);
            var entities = orderedResults.Skip((page-1)*perPage).Take(perPage).ToList();

            var result = entities.Select(q =>
            {
                var nomenclature = "";

                if (q.NomenclatureAlternativeId.HasValue)
                {
                    var clientName = q.NomenclatureAlternative?.Link.Customer != null
                        ? q.NomenclatureAlternative?.Link.Customer?.Name
                        : q.NomenclatureAlternative?.Link.Supplier?.Name;

                    nomenclature = $"{clientName}: {q.NomenclatureAlternative?.Name}";
                }

                if (q.NomenclatureId.HasValue)
                {
                    nomenclature = q.Nomenclature.Name;
                }

                return new UomFactorDataItem
                {
                    Id = q.Id,
                    Factor = q.FromUomId == uomId ? q.Factor : 1m / q.Factor,
                    Nomenclature = nomenclature,
                    Uom = q.FromUomId == uomId ? q.ToUom.Name : q.FromUom.Name,
                    FromId = q.FromUomId == uomId ? q.FromUomId : q.ToUomId,
                    ToId = q.FromUomId == uomId ? q.ToUomId : q.FromUomId,
                    NomenclatureAlternativeId = q.NomenclatureAlternativeId,
                    NomenclatureId = q.NomenclatureId
                };
            }).ToList();

            return new UomFactorData
            {
                Total = total,
                Data = result
            };
        }

        public async Task<UomDto> Create(Guid ownerId, string name, decimal? quantity = null)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            var entry = await _db.UnitsOfMeasurements.AddAsync(CreateEntity(name, ownerId, quantity, new List<string>()));
            await _db.SaveChangesAsync();
            return entry.Entity.Adapt<UomDto>();
        }

        public async Task<UomDto> Create(Guid companyId, string name, List<string> alternativeNames, decimal? quantity = null)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            var entry = await _db.UnitsOfMeasurements.AddAsync(CreateEntity(name, companyId, quantity, alternativeNames));
            await _db.SaveChangesAsync();
            return entry.Entity.Adapt<UomDto>();
        }

        public UomDto CreateOrUpdate(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var normalizedName = name.CustomNormalize();

            var entity =
                _db.UnitsOfMeasurements.FirstOrDefault(q =>
                    q.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                    || q.NormalizedName.Equals(normalizedName, StringComparison.InvariantCultureIgnoreCase) );

            if (entity == null)
            {
                var entry = _db.UnitsOfMeasurements.Add(CreateEntity(name, Guid.Empty, null, new List<string>()));
                _db.SaveChanges();
                entity = entry.Entity;
            }

            return entity.Adapt<UomDto>();
        }

        private UnitsOfMeasurement CreateEntity(string name, Guid ownerId, decimal? quantity, List<string> alternativeNames)
            => new UnitsOfMeasurement
            {
                Name = name.Trim(),
                NormalizedName = name.CustomNormalize(),
                OwnerId = ownerId,
                Quantity = quantity,
                Json =
                {
                    AlternativeNames = alternativeNames.Select(q => new UomAlternativeName{ Name = q }).ToList()
                }
            };

        public IEnumerable<UomDto> GetAll() => _db.UnitsOfMeasurements.Where(q => !q.IsDeleted).ProjectToType<UomDto>().ToList();

        public IEnumerable<UomDto> GetByNames(params string[] uomNames)
        {
            if (!uomNames.Any())
            {
                return Enumerable.Empty<UomDto>();
            }

            return (from item in _db.UnitsOfMeasurements
                    where !item.IsDeleted && uomNames.Contains(item.Name)
                    select item).ProjectToType<UomDto>().ToList();
        }

        public UomAutocompleteResponse Autocomplete(string s, Guid ownerId)
        {
            var response = new UomAutocompleteResponse
            {
                Items = new List<UomAutocompleteResponse.AutocompleteItem>()
            };

            if (string.IsNullOrEmpty(s)) return response;

            var cacheKey = Consts.CacheKeys.UomAutocomplete(ownerId, s);

            if (!_cache.TryGetValue(cacheKey, out List<UomAutocompleteResponse.AutocompleteItem> items))
            {
                var normalizedName = s.CustomNormalize();

                var qry = _db.Query<UomAutocomplete>()
                    .FromSql("SELECT * FROM UnitsOfMeasurements " +
                             "OUTER APPLY OPENJSON([Json], '$.AlternativeNames') " +
                             "WITH (AlternativeName nvarchar(255) '$.Name', NormalizedAlternativeName nvarchar(255) '$.NormalizedName')")
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .Where(q => q.OwnerId == ownerId && !q.IsDeleted);

                var autocompleteItems = qry
                    .Where(q => q.Name == s
                                || q.Name.Contains(s)
                                || q.AlternativeName == s
                                || q.AlternativeName.Contains(s)
                                || q.NormalizedName == normalizedName
                                || q.NormalizedAlternativeName == normalizedName)
                    .ToList()
                    .Select(q => new
                    {
                        q.Id, q.Name, q.NormalizedName,
                        IsFullMatch = q.Name == s || q.NormalizedName == normalizedName || q.AlternativeName == s || q.NormalizedAlternativeName == normalizedName
                    })
                    .OrderByDescending(q => q.IsFullMatch)
                    .GroupBy(q => q.Id)
                    .Select(q => q.FirstOrDefault())
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

        public void SaveConversionRate(
            Guid ownerId,
            Guid fromUomId,
            Guid toUomId,
            Guid? nomenclatureAlternativeId,
            decimal factorC, decimal factorN)
        {
            if (fromUomId == toUomId) return; // don't store in database, factor = 1

            var rateQry = _db.UomConversionRates.IgnoreQueryFilters().Where(q =>
                q.OwnerId == ownerId &&
                ((q.FromUomId == fromUomId && q.ToUomId == toUomId)
                 || (q.FromUomId == toUomId && q.ToUomId == fromUomId)));

            var isCommon = factorC > 0;

            var rate = isCommon
                ? rateQry.FirstOrDefault()
                : rateQry.FirstOrDefault(q => q.NomenclatureAlternativeId == nomenclatureAlternativeId);
            if (rate == null)
            {
                var newRate = new UomConversionRate { FromUomId = fromUomId, ToUomId = toUomId, OwnerId = ownerId };
                if (isCommon)
                {
                    newRate.Factor = factorC;
                }
                else
                {
                    newRate.Factor = factorN;
                    newRate.NomenclatureAlternativeId = nomenclatureAlternativeId;
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

        public void Delete(List<Guid> ids, Guid ownerId)
        {
            var uoms = _db.UnitsOfMeasurements
                .IgnoreQueryFilters()
                .Where(q => ids.Contains(q.Id) && q.OwnerId == ownerId)
                .ToList();

            foreach (var uom in uoms)
            {
                uom.IsDeleted = true;
            }

            _db.SaveChanges();
        }

        public UomDto GetById(Guid id)
        {
            var entity = _db.UnitsOfMeasurements.Find(id);

            return entity?.Adapt<UomDto>();
        }

        public UomDto Update(Guid id, string name)
        {
            var entity = _db.UnitsOfMeasurements.Find(id);
            entity.Name = name.Trim();
            entity.NormalizedName = name.CustomNormalize();
            _db.SaveChanges();
            return entity.Adapt<UomDto>();
        }

        public UomDto Update(Guid id, string name, List<string> alternativeNames)
        {
            var entity = _db.UnitsOfMeasurements.Find(id);
            entity.Name = name.Trim();
            if (entity.Json == null)
                entity.Json = new UomJsonData();
            entity.Json.AlternativeNames = alternativeNames.Select(q => new UomAlternativeName { Name = q }).ToList();
            _db.Entry(entity).Property(q => q.Json).IsModified = true;
            _db.SaveChanges();
            return entity.Adapt<UomDto>();
        }

        public void DeleteConversionRate(Guid id)
        {
            _db.Remove(_db.UomConversionRates.Find(id));
            _db.SaveChanges();
        }

        public async Task SetPackagingUom(Guid ownerId, Guid uomId)
        {
            var settings = await GetDefaultUomSettings(ownerId);
            settings.PackagingUomId = uomId;
            await _db.SaveChangesAsync();
        }

        public async Task<Guid> GetPackagingUom(Guid ownerId)
        {
            var settings = await GetDefaultUomSettings(ownerId);
            return settings.PackagingUomId;
        }

        public IEnumerable<string> GetAllNormalizedNames(Guid ownerId)
        {
            var cacheKey = Consts.CacheKeys.UomAllNormalizedNames(ownerId);

            if (!_cache.TryGetValue(cacheKey, out List<string> names))
            {
                names = _db.UnitsOfMeasurements
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .Where(q => q.OwnerId == ownerId).Select(q => q.NormalizedName).Distinct().ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(5));

                _cache.Set(cacheKey, names, cacheEntryOptions);
            }

            return names;
        }

        private async Task<DefaultUom> GetDefaultUomSettings(Guid ownerId)
        {
            var settings = await _db.DefaultUoms
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(q => q.OwnerId == ownerId);

            if (settings == null) settings = await CreateDefaultUomSettings(ownerId);
            return settings;
        }

        private async Task<DefaultUom> CreateDefaultUomSettings(Guid ownerId)
        {
            var entry = await _db.DefaultUoms.AddAsync(new DefaultUom { OwnerId = ownerId });
            await _db.SaveChangesAsync();
            return entry.Entity;
        }
    }
}

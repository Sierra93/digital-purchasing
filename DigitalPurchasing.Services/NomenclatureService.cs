using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core;
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
    public class NomenclatureService : INomenclatureService
    {
        private readonly ApplicationDbContext _db;
        private readonly INomenclatureCategoryService _categoryService;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;

        public NomenclatureService(
            ApplicationDbContext dbContext,
            INomenclatureCategoryService categoryService,
            IMemoryCache cache,
            ILogger<NomenclatureService> logger)
        {
            _db = dbContext;
            _categoryService = categoryService;
            _cache = cache;
            _logger = logger;
        }

        public NomenclatureIndexData GetData(
            int page,
            int perPage,
            string sortField,
            bool sortAsc,
            string search)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }

            var qry = _db.Nomenclatures.Where(q => !q.IsDeleted);

            if (!string.IsNullOrEmpty(search))
            {
                qry = qry.Where(q =>
                    ( q.Name.Contains(search) && !string.IsNullOrEmpty(q.Name) ) ||
                    ( q.NameEng.Contains(search) && !string.IsNullOrEmpty(q.NameEng) ));
            }

            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc ? "" : " DESC")}");
            var result = orderedResults.Skip((page - 1) * perPage).Take(perPage).ProjectToType<NomenclatureIndexDataItem>().ToList();

            foreach (var nomenclatureResult in result)
            {
                nomenclatureResult.CategoryFullName = _categoryService.FullCategoryName(nomenclatureResult.CategoryId);
            }

            return new NomenclatureIndexData
            {
                Total = total,
                Data = result
            };
        }

        public NomenclatureDetailsData GetDetailsData(Guid nomId, int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }

            var qry = _db.NomenclatureAlternatives.Where(q => q.NomenclatureId == nomId);
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc ? "" : " DESC")}");
            var result = orderedResults
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .ProjectToType<NomenclatureDetailsDataItem>().ToList();

            var ids = result.Select(q => q.Id).ToList();

            var links = _db.NomenclatureAlternativeLinks
                .Include(q => q.Customer)
                .Include(q => q.Supplier)
                .Where(q => ids.Contains(q.AlternativeId))
                .ToList();

            foreach (var link in links)
            {
                var alt = result.Find(q => q.Id == link.AlternativeId);
                alt.ClientName = link.CustomerId.HasValue ? link.Customer.Name : link.Supplier.Name;
                alt.ClientType = (int) (link.CustomerId.HasValue ? ClientType.Customer : ClientType.Supplier);
            }

            return new NomenclatureDetailsData
            {
                Total = total,
                Data = result
            };
        }

        public NomenclatureVm CreateOrUpdate(NomenclatureVm vm)
        {
            var oldEntity = _db.Nomenclatures.FirstOrDefault(q => q.Name.Equals(vm.Name, StringComparison.InvariantCultureIgnoreCase));
            if (oldEntity != null)
            {
                oldEntity.Code = vm.Code;
                oldEntity.BatchUomId = vm.BatchUomId;
                oldEntity.MassUomId = vm.MassUomId;
                oldEntity.ResourceUomId = vm.ResourceUomId;
                oldEntity.ResourceBatchUomId = vm.ResourceBatchUomId;
                oldEntity.ResourceUomValue = vm.ResourceUomValue;
                oldEntity.MassUomValue = vm.MassUomValue;
                _db.SaveChanges();
                return oldEntity.Adapt<NomenclatureVm>();
            }

            var entity = vm.Adapt<Nomenclature>();
            var entry = _db.Nomenclatures.Add(entity);
            _db.SaveChanges();
            var result = entry.Entity.Adapt<NomenclatureVm>();
            return result;
        }

        public void CreateOrUpdate(List<NomenclatureVm> nomenclatures, Guid ownerId)
        {
            var allNames = _db.Nomenclatures
                .AsQueryable()
                .GroupBy(q => q.Name)
                .Select(q => new { Name = q.First().Name.ToLowerInvariant(), q.First().Id })
                .ToList()
                .DistinctBy(q => q.Name)
                .ToDictionary(q => q.Name, w => w.Id);

            var entities = nomenclatures.Adapt<List<Nomenclature>>();
            foreach (var entity in entities)
            {
                var normName = entity.Name.ToLowerInvariant();
                entity.Id = allNames.ContainsKey(normName) ? allNames[normName] : Guid.NewGuid();
                entity.OwnerId = ownerId;
            }
            _db.BulkInsertOrUpdate(entities);
        }

        public NomenclatureVm GetById(Guid id)
        {
            var entity = _db.Nomenclatures.Find(id);
            var result = entity.Adapt<NomenclatureVm>();
            return result;
        }

        public bool Update(NomenclatureVm model)
        {
            var entity = _db.Nomenclatures.Find(model.Id);
            if (entity == null) return false;

            entity.CategoryId = model.CategoryId;
            entity.Code = model.Code;
            entity.Name = model.Name;
            entity.NameEng = model.NameEng;

            entity.ResourceUomId = model.ResourceUomId;
            entity.ResourceUomValue = model.ResourceUomValue;
            entity.ResourceBatchUomId = model.ResourceBatchUomId;

            entity.BatchUomId = model.BatchUomId;

            entity.MassUomId = model.MassUomId;

            entity.MassUomValue = model.MassUomValue;

            _db.SaveChanges();
            return true;
        }

        public NomenclatureAutocompleteResult Autocomplete(AutocompleteOptions options)
        {
            if (options.OwnerId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(options.OwnerId));
            }

            var result = new NomenclatureAutocompleteResult
            {
                Items = new List<NomenclatureAutocompleteResult.AutocompleteResultItem>()
            };

            var q = options.Query.Trim();

            if (string.IsNullOrEmpty(q)) return result;

            var strComparison = StringComparison.InvariantCultureIgnoreCase;

            var cacheKey = Consts.CacheKeys.NomenclatureAutocomplete(options.OwnerId);

            if (!_cache.TryGetValue(cacheKey, out List<Nomenclature> ownerNomenclatures))
            {
                ownerNomenclatures = _db.Nomenclatures
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .AsQueryable()
                    .Include(w => w.BatchUom)
                    .Where(w => !w.IsDeleted && w.OwnerId == options.OwnerId)
                    .ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(5));

                _cache.Set(cacheKey, ownerNomenclatures, cacheEntryOptions);
            }
            else
            {
                _logger.LogInformation($"Autocomplete cache hit - {cacheKey}");
            }

            var mainResults = ownerNomenclatures
                .Where(w =>
                    (!string.IsNullOrEmpty(w.Name) && w.Name.Contains(q, strComparison)) ||
                    (!string.IsNullOrEmpty(w.NameEng) && w.NameEng.Contains(q, strComparison)) ||
                    (!string.IsNullOrEmpty(w.Code) && w.Code.Contains(q, strComparison)))
                .ToList();

            if (options.SearchInAlts)
            {
                var altCacheKey =
                    Consts.CacheKeys.NomenclatureAutocompleteSearchInAlts(options.OwnerId, options.ClientId);

                if (!_cache.TryGetValue(altCacheKey,
                    out List<NomenclatureAlternative> altNoms))
                {
                    var altNomsQry = _db.NomenclatureAlternatives
                        .AsNoTracking()
                        .IgnoreQueryFilters()
                        .Include(r => r.Link)
                        .Where(n => n.OwnerId == options.OwnerId)
                        .AsQueryable();

                    altNomsQry = options.ClientType == ClientType.Customer
                        ? altNomsQry.Where(r => r.Link.CustomerId == options.ClientId)
                        : altNomsQry.Where(r => r.Link.SupplierId == options.ClientId);

                    altNoms = altNomsQry.ToList();

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(5));

                    _cache.Set(altCacheKey, altNoms, cacheEntryOptions);
                }
                else
                {
                    _logger.LogInformation($"Autocomplete cache hit - {altCacheKey}");
                }

                var altNomIds = altNoms.Where(w => w.Name.Contains(q, strComparison))
                    .Select(w => w.NomenclatureId)
                    .Distinct()
                    .ToList();

                if (altNomIds.Any())
                {
                    var mainNomIds = mainResults.Select(w => w.Id);
                    var altResults = _db.Nomenclatures.IgnoreQueryFilters().Where(w => w.OwnerId == options.OwnerId && altNomIds.Contains(w.Id) && !mainNomIds.Contains(w.Id) && !w.IsDeleted).ToList();
                    mainResults = mainResults.Union(altResults).ToList();
                }
            }

            result.Items.AddRange(mainResults.Adapt<List<NomenclatureAutocompleteResult.AutocompleteResultItem>>());

            return result;
        }

        public BaseResult<NomenclatureAutocompleteResult.AutocompleteResultItem> AutocompleteSingle(Guid id)
        {
            var entity = _db.Nomenclatures.AsNoTracking().Include(w => w.BatchUom).FirstOrDefault(q => q.Id == id);
            var data = entity?.Adapt<NomenclatureAutocompleteResult.AutocompleteResultItem>();
            return new BaseResult<NomenclatureAutocompleteResult.AutocompleteResultItem>(data);
        }

        public void Delete(Guid id)
        {
            var entity = _db.Nomenclatures.Find(id);
            if (entity == null) return;
            entity.IsDeleted = true;
            _db.SaveChanges();
        }

        public void AddNomenclatureForCustomer(Guid prItemId)
        {
            var prItem = _db.PurchaseRequestItems.Include(q => q.PurchaseRequest).First(q => q.Id == prItemId);
            if (prItem.NomenclatureId.HasValue && prItem.PurchaseRequest.CustomerId.HasValue)
            {
                AddOrUpdateNomenclatureAlts(
                    prItem.PurchaseRequest.OwnerId,
                    prItem.PurchaseRequest.CustomerId.Value,
                    ClientType.Customer,
                    prItem.NomenclatureId.Value,
                    prItem.RawName,
                    prItem.RawCode,
                    prItem.RawUomMatchId);
            }
        }

        public void AddNomenclatureForSupplier(Guid soItemId)
        {
            var soItem = _db.SupplierOfferItems.IgnoreQueryFilters().Include(q => q.SupplierOffer).First(q => q.Id == soItemId);
            if (soItem.NomenclatureId.HasValue && soItem.SupplierOffer.SupplierId.HasValue)
            {
                AddOrUpdateNomenclatureAlts(
                    soItem.SupplierOffer.OwnerId,
                    soItem.SupplierOffer.SupplierId.Value,
                    ClientType.Supplier,
                    soItem.NomenclatureId.Value,
                    soItem.RawName,
                    soItem.RawCode,
                    soItem.RawUomId);
            }
        }

        public NomenclatureAlternativeVm GetAlternativeById(Guid id)
        {
            var entity = _db.NomenclatureAlternatives.Find(id);
            var result = entity.Adapt<NomenclatureAlternativeVm>();
            var link = _db.NomenclatureAlternativeLinks
                .Include(q => q.Supplier)
                .Include(q => q.Customer)
                .FirstOrDefault(q => q.AlternativeId == id);
            if (link != null)
            {
                result.ClientName = link.CustomerId.HasValue
                    ? link.Customer.Name
                    : link.Supplier.Name;
                result.ClientType = link.CustomerId.HasValue
                    ? ClientType.Customer
                    : ClientType.Supplier;
            }

            return result;
        }

        public void UpdateAlternative(NomenclatureAlternativeVm model)
        {
            var entity = _db.NomenclatureAlternatives.Find(model.Id);

            //entity.ClientName = model.ClientName;
            //entity.ClientType = model.ClientType;
            entity.Name = model.Name;
            entity.Code = model.Code;

            entity.ResourceUomId = model.ResourceUomId;
            entity.ResourceUomValue = model.ResourceUomValue;

            entity.BatchUomId = model.BatchUomId;
            entity.ResourceBatchUomId = model.ResourceBatchUomId;

            entity.MassUomId = model.MassUomId;
            entity.MassUomValue = model.MassUomValue;

            _db.SaveChanges();
        }

        public void AddOrUpdateNomenclatureAlts(Guid ownerId, Guid clientId, ClientType clientType,
            Guid nomenclatureId, string name, string code, Guid? uom)
            => AddOrUpdateNomenclatureAlts(ownerId, clientId, clientType,
                new List<(Guid NomenclatureId, string Name, string Code, Guid? Uom)>
                {
                    (NomenclatureId:nomenclatureId, Name:name, Code:code, Uom: uom)
                });

        public void AddOrUpdateNomenclatureAlts(
            Guid ownerId,
            Guid clientId,
            ClientType clientType,
            List<(Guid NomenclatureId, string Name, string Code, Guid? Uom)> alts)
        {
            var altNomenclaturesQry = _db.NomenclatureAlternatives
                .Include(q => q.Link)
                .IgnoreQueryFilters()
                .Where(q => q.OwnerId == ownerId);

            altNomenclaturesQry = clientType == ClientType.Customer
                ? altNomenclaturesQry.Where(q => q.Link.CustomerId == clientId)
                : altNomenclaturesQry.Where(q => q.Link.SupplierId == clientId);

            var altNomenclatures = altNomenclaturesQry.ToList();

            var forBulkInsertNA = new List<NomenclatureAlternative>();
            var forBulkUpdateNA = new List<NomenclatureAlternative>();
            var forBulkInsertLink = new List<NomenclatureAlternativeLink>();
            
            foreach (var alt in alts)
            {
                var altName = altNomenclatures.FirstOrDefault(q =>
                    q.NomenclatureId == alt.NomenclatureId &&
                    q.Name.Equals(alt.Name, StringComparison.InvariantCultureIgnoreCase));

                if (altName != null)
                {
                    if (!string.IsNullOrEmpty(altName.Code) && altName.BatchUomId.HasValue) continue;

                    if (string.IsNullOrEmpty(altName.Code))
                    {
                        altName.Code = alt.Code;
                    }

                    if (!altName.BatchUomId.HasValue)
                    {
                        altName.BatchUomId = alt.Uom;
                    }

                    forBulkUpdateNA.Add(altName);
                }
                else
                {
                    var naId = Guid.NewGuid();
                    altName = new NomenclatureAlternative
                    {
                        Id = naId,
                        Name = alt.Name,
                        Code = alt.Code,
                        BatchUomId = alt.Uom,
                        NomenclatureId = alt.NomenclatureId,
                        OwnerId = ownerId
                    };

                    var link = new NomenclatureAlternativeLink
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = clientType == ClientType.Customer ? clientId : (Guid?) null,
                        SupplierId = clientType == ClientType.Supplier ? clientId : (Guid?) null,
                        AlternativeId = naId
                    };

                    forBulkInsertNA.Add(altName);
                    forBulkInsertLink.Add(link);
                }
            }

            if (forBulkInsertNA.Any())
            {
                _db.BulkInsert(forBulkInsertNA);
                _db.BulkInsert(forBulkInsertLink);
            }

            if (forBulkUpdateNA.Any())
                _db.BulkUpdate(forBulkUpdateNA);
        }

        // todo: add owner id?
        private void AddAlternative(Guid nomenclatureId, Guid clientId, ClientType clientType, string name, string code, Guid? uom)
        {
            name = name.Trim();
            code = code.Trim();

            var altNamesQry = _db.NomenclatureAlternatives
                .Include(q => q.Link)
                .AsQueryable();

            altNamesQry = clientType == ClientType.Customer
                ? altNamesQry.Where(q => q.Link.CustomerId == clientId)
                : altNamesQry.Where(q => q.Link.SupplierId == clientId);

            var altName = altNamesQry.FirstOrDefault(q =>
                q.NomenclatureId == nomenclatureId && q.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (altName != null)
            {
                if (string.IsNullOrEmpty(altName.Code))
                    altName.Code = code;
                if (!altName.BatchUomId.HasValue)
                {
                    altName.BatchUomId = uom;
                }
            }
            else
            {
                _db.NomenclatureAlternatives.Add(new NomenclatureAlternative
                {
                    Name = name,
                    Code = code,
                    BatchUomId = uom,
                    NomenclatureId = nomenclatureId,
                    Link = new NomenclatureAlternativeLink
                    {
                        CustomerId = clientType == ClientType.Customer ? clientId : (Guid?)null,
                        SupplierId = clientType == ClientType.Supplier ? clientId : (Guid?)null
                    }
                });
            }

            _db.SaveChanges();
        }
    }
}

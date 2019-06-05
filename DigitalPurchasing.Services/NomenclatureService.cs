using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using DigitalPurchasing.Services.Exceptions;
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
        private readonly INomenclatureComparisonService _nomenclatureComparisonService;

        public NomenclatureService(
            ApplicationDbContext dbContext,
            INomenclatureCategoryService categoryService,
            IMemoryCache cache,
            ILogger<NomenclatureService> logger,
            INomenclatureComparisonService nomenclatureComparisonService)
        {
            _db = dbContext;
            _categoryService = categoryService;
            _cache = cache;
            _logger = logger;
            _nomenclatureComparisonService = nomenclatureComparisonService;
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

            search = search?.Trim();

            if (!string.IsNullOrEmpty(search))
            {
                qry = from q in qry
                      where (q.Name.Contains(search)) ||
                            (q.NameEng.Contains(search)) ||
                            (q.Code.Contains(search)) ||
                            q.Alternatives.Any(na => na.Name.Contains(search) || na.Code.Contains(search))
                      select q;
            }

            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc ? "" : " DESC")}");
            var result = orderedResults.Skip((page - 1) * perPage).Take(perPage).ProjectToType<NomenclatureIndexDataItem>().ToList();

            foreach (var item in result)
            {
                item.CategoryFullName = _categoryService.FullCategoryName(item.CategoryId);
                item.HasAlternativeWithRequiredName = !string.IsNullOrWhiteSpace(search) &&
                    !item.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase) &&
                    item.NameEng?.Contains(search, StringComparison.InvariantCultureIgnoreCase) != true &&
                    item.Code?.Contains(search, StringComparison.InvariantCultureIgnoreCase) != true;
            }

            return new NomenclatureIndexData
            {
                Total = total,
                Data = result
            };
        }

        public NomenclatureWholeData GetWholeNomenclature()
        {
            var qry = from n in _db.Nomenclatures.Where(q => !q.IsDeleted)
                      join na in _db.NomenclatureAlternatives on n.Id equals na.NomenclatureId into g
                      from possibleNa in g.DefaultIfEmpty()
                      join nal in _db.NomenclatureAlternativeLinks.Include(nal1 => nal1.Supplier).Include(na1 => na1.Customer) on possibleNa.Id equals nal.AlternativeId into g2
                      from possibleNal in g2.DefaultIfEmpty()
                      select new
                      {
                          nomId = n.Id,
                          nomName = n.Name,
                          nomNameEng = n.NameEng,
                          nomCode = n.Code,
                          nomCatName = n.Category.Name,
                          batchUomName = n.BatchUom.Name,
                          n.MassUomValue,
                          massUomName = n.MassUom.Name,
                          packUomName = n.PackUom.Name,
                          n.PackUomValue,
                          resourceUomName = n.ResourceUom.Name,
                          n.ResourceUomValue,
                          resourceBatchUomName = n.ResourceBatchUom.Name,
                          alt = possibleNa == null
                              ? null
                              : new
                              {
                                  code = possibleNa.Code,
                                  name = possibleNa.Name,
                                  batchUomName = possibleNa.BatchUom.Name,
                                  possibleNa.MassUomValue,
                                  massUomName = possibleNa.MassUom.Name,
                                  resourceUomName = possibleNa.ResourceUom.Name,
                                  possibleNa.ResourceUomValue,
                                  resourceBatchUomName = possibleNa.ResourceBatchUom.Name,
                                  packUomName = possibleNa.PackUom.Name,
                                  possibleNa.PackUomValue,
                              },
                          customer = possibleNal.Customer,
                          supplier = possibleNal.Supplier
                      };

            var qryResult = qry.ToList();

            var result = new NomenclatureWholeData();

            foreach (var item in qryResult.GroupBy(_ => _.nomId))
            {
                var nom = item.First(_ => _.nomId == item.Key);
                var nomenclature = new NomenclatureIndexDataItem()
                {
                    CategoryName = nom.nomCatName,
                    Id = nom.nomId,
                    Code = nom.nomCode,
                    Name = nom.nomName,
                    NameEng = nom.nomNameEng,
                    BatchUomName = nom.batchUomName,
                    MassUomName = nom.massUomName,
                    MassUomValue = nom.MassUomValue,
                    ResourceUomName = nom.resourceUomName,
                    ResourceUomValue = nom.ResourceUomValue,
                    ResourceBatchUomName = nom.resourceBatchUomName,
                    PackUomName = nom.packUomName,
                    PackUomValue = nom.PackUomValue
                };
                var alternatives = new NomenclatureDetailsData()
                {
                    Data = item.Where(_ => _.alt != null && (_.customer != null || _.supplier != null)).Select(_ => new NomenclatureDetailsDataItem()
                    {
                        Name = _.alt.name,
                        Code = _.alt.code,
                        BatchUomName = _.alt.batchUomName,
                        MassUomName = _.alt.massUomName,
                        MassUomValue = _.alt.MassUomValue,
                        ResourceUomName = _.alt.resourceUomName,
                        ResourceUomValue = _.alt.ResourceUomValue,
                        ResourceBatchUomName = _.alt.resourceBatchUomName,
                        PackUomName = _.alt.packUomName,
                        PackUomValue = _.alt.PackUomValue,
                        ClientName = _.customer?.Id != null ? _.customer?.Name : _.supplier?.Name,
                        ClientType = (int)(_.customer?.Id != null ? ClientType.Customer : ClientType.Supplier),
                        ClientPublicId = _.customer?.Id != null ? _.customer?.PublicId : _.supplier?.PublicId
                    }).ToList()
                };
                result.Nomenclatures.Add(nomenclature, alternatives);
            }

            return result;
        }

        public NomenclatureDetailsData GetDetailsData(Guid nomId, int page, int perPage, string sortField, bool sortAsc, string sortBySearch)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }

            var qry = _db.NomenclatureAlternatives.Where(q => q.NomenclatureId == nomId);
            var total = qry.Count();

            sortBySearch = sortBySearch?.Trim();

            var orderedResults = string.IsNullOrEmpty(sortBySearch)
                ? qry.OrderBy($"{sortField}{(sortAsc ? "" : " DESC")}")
                : qry.OrderByDescending(q => q.Name.Contains(sortBySearch))
                        .ThenByDescending(q => q.Code.Contains(sortBySearch))
                        .ThenBy(q => q.Name);
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

        public NomenclatureVm CreateOrUpdate(NomenclatureVm vm, Guid ownerId)
        {
            if (HasSameNomenclatureName(vm.Id == default ? (Guid?)null : vm.Id, vm.Name?.Trim()))
            {
                throw new SameNomenclatureNameException();
            }

            var entity = _db.Nomenclatures.FirstOrDefault(q => q.Id == vm.Id);

            string name = vm.Name?.Trim();
            var isNew = entity == null;
            bool nameIsChanged = false;

            if (isNew)
            {
                entity = new Nomenclature()
                {
                    OwnerId = ownerId
                };
                _db.Nomenclatures.Add(entity);
            }
            else
            {
                nameIsChanged = entity.Name != name;
            }

            entity.CategoryId = vm.CategoryId;
            entity.Code = vm.Code?.Trim();
            entity.Name = name;
            entity.NameEng = vm.NameEng?.Trim();

            entity.ResourceUomId = vm.ResourceUomId;
            entity.ResourceUomValue = vm.ResourceUomValue;
            entity.ResourceBatchUomId = vm.ResourceBatchUomId;

            entity.BatchUomId = vm.BatchUomId;

            entity.MassUomId = vm.MassUomId;
            entity.MassUomValue = vm.MassUomValue;

            entity.PackUomId = vm.PackUomId;
            entity.PackUomValue = vm.PackUomValue;

            _db.SaveChanges();

            var cd = GetComparisonDataByNomenclatureName(entity.Name);

            if (nameIsChanged)
            {
                _db.NomenclatureComparisonDatas.Remove(
                    _db.NomenclatureComparisonDatas.First(_ => _.NomenclatureId == entity.Id && !_.NomenclatureAlternativeId.HasValue));
                _db.SaveChanges();
            }

            if (isNew || nameIsChanged)
            {
                cd.Id = Guid.NewGuid();
                cd.NomenclatureId = entity.Id;
                _db.NomenclatureComparisonDatas.Add(cd);
                cd.AdjustedNameNgrams.AddRange(GetNgramsForNomComparisonData(cd, ownerId));
                _db.SaveChanges();
            }

            return entity.Adapt<NomenclatureVm>();
        }

        private NomenclatureComparisonData GetComparisonDataByNomenclatureName(string nomName)
        {
            var terms = _nomenclatureComparisonService.CalculateComparisonTerms(nomName);

            return new NomenclatureComparisonData
            {
                AdjustedNomenclatureDigits = terms.AdjustedDigits,
                AdjustedNomenclatureName = terms.AdjustedName,
                NomenclatureDimensions = terms.NomDimensions
            };
        }

        public void CreateOrUpdate(List<NomenclatureVm> nomenclatures, Guid ownerId)
        {
            var allNames = _db.Nomenclatures.IgnoreQueryFilters()
                .Where(q => q.OwnerId == ownerId)
                .Select(q => new
                {
                    q.Name,
                    q.Id
                })
                .ToList();

            var newEntities = new List<Nomenclature>();
            var entities = nomenclatures.Adapt<List<Nomenclature>>();
            foreach (var entity in entities)
            {
                var normName = entity.Name.ToLowerInvariant();
                var existed = allNames.FirstOrDefault(i => i.Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase));
                entity.Id = existed?.Id ?? Guid.NewGuid();
                if (existed == null)
                {
                    newEntities.Add(entity);
                }
                entity.OwnerId = ownerId;
            }
            _db.BulkInsertOrUpdate(entities);

            if (newEntities.Any())
            {
                var compDataItems = new List<NomenclatureComparisonData>();
                newEntities.ForEach(n =>
                {
                    var compDataItem = GetComparisonDataByNomenclatureName(n.Name);
                    compDataItem.NomenclatureId = n.Id;
                    compDataItems.Add(compDataItem);
                });
                _db.BulkInsert(compDataItems);
                _db.BulkInsert(compDataItems.Select(cd => GetNgramsForNomComparisonData(cd, ownerId)).SelectMany(ng => ng).ToList());
            }
        }

        private IEnumerable<NomenclatureComparisonDataNGram> GetNgramsForNomComparisonData(NomenclatureComparisonData cd, Guid ownerId)
        {
            byte ngramLen = 3;
            return from ng in cd.AdjustedNomenclatureName.Ngrams(ngramLen)
                   select new NomenclatureComparisonDataNGram()
                   {
                       NomenclatureComparisonDataId = cd.Id,
                       N = ngramLen,
                       Gram = ng,
                       OwnerId = ownerId
                   };
        }

        public NomenclatureVm GetById(Guid id, bool globalSearch = false)
        {
            var qry = _db.Nomenclatures.AsQueryable();
            if (globalSearch)
            {
                qry = qry.IgnoreQueryFilters();
            }

            var entity = qry.First(q => q.Id == id);
            var result = entity.Adapt<NomenclatureVm>();
            return result;
        }

        private bool HasSameNomenclatureName(Guid? exceptNomenclatureId, string name) =>
            !string.IsNullOrWhiteSpace(name) &&
            _db.Nomenclatures.Any(_ => _.Id != exceptNomenclatureId && _.Name == name);

        public NomenclatureVm FindBestFuzzyMatch(Guid ownerId, string nomName)
        {
            // Ngrams with specified length should already exist in the database
            const byte ngramLen = 3;

            const int maxNameLevenshteinDistance = 30;

            var sw = System.Diagnostics.Stopwatch.StartNew();

            var match = FindBestFuzzyMatch(ownerId, nomName, maxNameLevenshteinDistance, ngramLen, 15);
            if (match == null)
            {
                match = FindBestFuzzyMatch(ownerId, nomName, maxNameLevenshteinDistance, ngramLen, 10);
            }
            if (match == null)
            {
                match = FindBestFuzzyMatch(ownerId, nomName, maxNameLevenshteinDistance, ngramLen, 3);
            }
            sw.Stop();

            return match;
        }

        private NomenclatureVm FindBestFuzzyMatch(Guid ownerId, string nomName, int maxNameLevenshteinDistance,
            byte ngramLen, byte minNgramIntersect)
        {
            var compTerms = _nomenclatureComparisonService.CalculateComparisonTerms(nomName);
            var ngrams = compTerms.AdjustedName.Ngrams(ngramLen).ToList();

            var satisfiedNgrams = (from item in _db.NomenclatureComparisonDataNGrams
                                   let nom = item.NomenclatureComparisonData.Nomenclature
                                   where item.OwnerId == ownerId &&
                                        ngrams.Contains(item.Gram) &&
                                        !nom.IsDeleted
                                   group item by item.NomenclatureComparisonDataId into g
                                   where g.Count() >= minNgramIntersect
                                   select g.Key).Distinct();
            var comparisonDataQry = from ncdId in satisfiedNgrams
                                    join ncd in _db.NomenclatureComparisonDatas.Include(_ => _.Nomenclature) on ncdId equals ncd.Id
                                    select ncd;

            var ncDataItems = comparisonDataQry.ToList();

            var results = from cd in ncDataItems
                          let isAnalog = cd.NomenclatureAlternativeId.HasValue
                          let distance = _nomenclatureComparisonService.CalculateDistance(compTerms, new NomenclatureComparisonTerms()
                          {
                              AdjustedDigits = cd.AdjustedNomenclatureDigits,
                              AdjustedName = cd.AdjustedNomenclatureName,
                              NomDimensions = cd.NomenclatureDimensions
                          })
                          orderby distance.CompleteDistance, isAnalog
                          select new
                          {
                              isAnalog,
                              cd.Nomenclature,
                              distance
                          };

            var match = results.FirstOrDefault()?.Nomenclature.Adapt<NomenclatureVm>();

            return match;

            //var results = compTerms.NomDimensions == null
            //    ? from cd in comparisonDataQry
            //      let nameDistance = ApplicationDbContext.LevenshteinDistanceFunc(compTerms.AdjustedName, cd.AdjustedNomenclatureName, maxNameLevenshteinDistance)
            //      let digitsDistance = ApplicationDbContext.LevenshteinDistanceFunc(compTerms.AdjustedDigits, cd.AdjustedNomenclatureDigits, maxNameLevenshteinDistance) ?? maxNameLevenshteinDistance
            //      //let maxSubstringLen = ApplicationDbContext.LongestCommonSubstringLenFunc(compTerms.AdjustedName, cd.AdjustedNomenclatureName)
            //      let distance = nameDistance + digitsDistance /*- 2 * maxSubstringLen*/
            //      where nameDistance.HasValue
            //      orderby distance
            //      select cd.Nomenclature
            //    : from cd in comparisonDataQry
            //      let nameDistance = string.IsNullOrEmpty(cd.NomenclatureDimensions)
            //          ? ApplicationDbContext.LevenshteinDistanceFunc(compTerms.AdjustedName, cd.AdjustedNomenclatureName, maxNameLevenshteinDistance)
            //          : ApplicationDbContext.LevenshteinDistanceFunc(compTerms.AdjustedNameWithDimensions, cd.AdjustedNomenclatureNameWithDimensions, maxNameLevenshteinDistance)
            //      let digitsDistance = ApplicationDbContext.LevenshteinDistanceFunc(compTerms.AdjustedDigits, cd.AdjustedNomenclatureDigits, maxNameLevenshteinDistance) ?? maxNameLevenshteinDistance
            //      //let maxSubstringLen = string.IsNullOrEmpty(cd.NomenclatureDimensions)
            //      //    ? ApplicationDbContext.LongestCommonSubstringLenFunc(compTerms.AdjustedName, cd.AdjustedNomenclatureName)
            //      //    : ApplicationDbContext.LongestCommonSubstringLenFunc(compTerms.AdjustedNameWithDimensions, cd.AdjustedNomenclatureNameWithDimensions)
            //      let distance = nameDistance + digitsDistance /*- 2 * maxSubstringLen*/
            //      where nameDistance.HasValue
            //      orderby distance
            //      select cd.Nomenclature;
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
                    (!string.IsNullOrEmpty(w.Name) && w.Name.Trim().Contains(q, strComparison)) ||
                    (!string.IsNullOrEmpty(w.NameEng) && w.NameEng.Trim().Contains(q, strComparison)) ||
                    (!string.IsNullOrEmpty(w.Code) && w.Code.Trim().Contains(q, strComparison)))
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

            var resultItems = mainResults.Select(n => new NomenclatureAutocompleteResult.AutocompleteResultItem
            {
                Id = n.Id,
                Code = n.Code,
                Name = n.Name,
                NameEng = n.NameEng,
                BatchUomId = n.BatchUom?.Id ?? Guid.Empty,
                BatchUomName = n.BatchUom?.Name ?? string.Empty,                
                IsFullMatch = (!string.IsNullOrEmpty(n.Name) && n.Name.Trim().Equals(q, strComparison))
                    || (!string.IsNullOrEmpty(n.NameEng) && n.NameEng.Trim().Equals(q, strComparison))
                    || (!string.IsNullOrEmpty(n.Code) && n.Code.Trim().Equals(q, strComparison))
            })
            .OrderByDescending(w => w.IsFullMatch)
            .ThenByDescending(w => !string.IsNullOrEmpty(w.Name) && w.Name.Trim().StartsWith(q, strComparison))
            .ThenByDescending(w => !string.IsNullOrEmpty(w.NameEng) && w.NameEng.Trim().StartsWith(q, strComparison))
            .ThenByDescending(w => !string.IsNullOrEmpty(w.Code) && w.Code.Trim().StartsWith(q, strComparison))
            .ToList();

            result.Items.AddRange(resultItems);

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

        public IEnumerable<NomenclatureVm> GetByNames(params string[] nomenclatureNames)
        {
            if (!nomenclatureNames.Any())
            {
                return Enumerable.Empty<NomenclatureVm>();
            }

            var nomenclatures = (from item in _db.Nomenclatures
                                 where !item.IsDeleted &&
                                     nomenclatureNames.Contains(item.Name)
                                 select item).ToList();

            return nomenclatures.Select(_ => _.Adapt<NomenclatureVm>());
        }
    }
}

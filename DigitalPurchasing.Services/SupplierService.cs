using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using DigitalPurchasing.Services.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DigitalPurchasing.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext _db;
        private const StringComparison StrComparison = StringComparison.InvariantCultureIgnoreCase;
        private readonly INomenclatureCategoryService _categoryService;
        private readonly ICounterService _counterService;
        private readonly IMemoryCache _memoryCache;

        public SupplierService(
            ApplicationDbContext db,
            INomenclatureCategoryService categoryService,
            ICounterService counterService,
            IMemoryCache memoryCache)
        {
            _db = db;
            _categoryService = categoryService;
            _counterService = counterService;
            _memoryCache = memoryCache;
        }

        public SupplierAutocomplete Autocomplete(AutocompleteBaseOptions options, bool includeCategories)
        {
            var entities = _db.Suppliers
                .Where(q => !string.IsNullOrEmpty(q.Name))
                .Where(q => q.Name.Equals(options.Query, StrComparison) || q.Name.Contains(options.Query, StrComparison))
                .OrderBy(q => q.Name)
                .Select(q => new SupplierAutocomplete.Supplier { Name = q.Name, Id = q.Id })
                .ToList();

            var result = new SupplierAutocomplete();

            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    if (includeCategories)
                    {
                        entity.Categories = GetSupplierNomenclatureCategories(entity.Id);
                    }
                    result.Items.Add(entity);
                }
            }

            return result;
        }

        public Guid CreateSupplier(string name, Guid ownerId)
        {
            var entry = _db.Suppliers.Add(new Supplier { Name = name, PublicId = _counterService.GetSupplierNextId(ownerId) });
            _db.SaveChanges();
            return entry.Entity.Id;
        }

        public string GetNameById(Guid id) => _db.Suppliers.Find(id).Name;

        public SupplierIndexData GetData(int page, int perPage, string sortField, bool sortAsc, string search)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }

            var qry = _db.Suppliers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                qry = qry.Where(q =>
                    !string.IsNullOrEmpty(q.Name) && q.Name.Contains(search));
            }

            var total = qry.Count();
            qry = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}")
                .Skip((page - 1) * perPage)
                .Take(perPage);
            var result = qry.ProjectToType<SupplierIndexDataItem>().ToList();

            var supplier2altNomCategories = (from s in qry
                                             join nal in _db.NomenclatureAlternativeLinks on s.Id equals nal.SupplierId
                                             join na in _db.NomenclatureAlternatives on nal.AlternativeId equals na.Id
                                             join n in _db.Nomenclatures on na.NomenclatureId equals n.Id
                                             where !n.IsDeleted
                                             group new
                                             {
                                                 supplierId = s.Id,
                                                 altCategoryId = n.CategoryId
                                             } by new { s.Id, altCategoryId = n.CategoryId } into g
                                             select g.FirstOrDefault()).ToList();


            foreach (var item in result)
            {                
                var supplierCategoryIds = supplier2altNomCategories
                    .Where(_ => _.supplierId == item.Id)
                    .Select(_ => _.altCategoryId)
                    .ToList();

                var uniqueCategoryIds = new List<Guid>();

                foreach (var categoryHierarchy in supplierCategoryIds.Select(cId => _categoryService.GetCategoryHierarchy(cId)))
                {
                    foreach (var categoryInfo in categoryHierarchy)
                    {
                        if (uniqueCategoryIds.Contains(categoryInfo.Id)) continue;
                        uniqueCategoryIds.Add(categoryInfo.Id);
                        break;
                    }
                }

                item.MainCategoriesCsv = string.Join(", ", uniqueCategoryIds.Select(cId => _categoryService.GetById(cId).Name));
            }

            return new SupplierIndexData
            {
                Total = total,
                Data = result,                
            };
        }

        public SupplierVm GetById(Guid id) => GetById(id, false);

        public SupplierVm GetById(Guid id, bool globalSearch)
        {
            var qry = _db.Suppliers.AsQueryable();
            if (globalSearch) qry = qry.IgnoreQueryFilters();

            return qry.FirstOrDefault(q => q.Id == id)?.Adapt<SupplierVm>();
        }

        public List<SupplierContactPersonVm> GetContactPersonsBySupplier(Guid supplierId, bool whichCouldBeUsedForRequestsOnly = false)
        {
            var query = _db.SupplierContactPersons
                .IgnoreQueryFilters()
                .Where(q => q.SupplierId == supplierId);

            if (whichCouldBeUsedForRequestsOnly)
            {
                query = query.Where(q => q.UseForRequests);
            }

            return query
                .ProjectToType<SupplierContactPersonVm>()
                .ToList();
        }

        public Guid AddContactPerson(SupplierContactPersonVm vm)
        {
            var contactPerson = vm.Adapt<SupplierContactPerson>();
            contactPerson.PhoneNumber = vm.PhoneNumber.CleanPhoneNumber();
            contactPerson.MobilePhoneNumber = vm.MobilePhoneNumber.CleanPhoneNumber()?.LastSymbols(10);
            var entry = _db.SupplierContactPersons.Add(contactPerson);
            _db.SaveChanges();
            return entry.Entity.Id;
        }

        public Guid EditContactPerson(SupplierContactPersonVm vm)
        {
            var entity = _db.SupplierContactPersons.Find(vm.Id);
            entity.Email = vm.Email;
            entity.FirstName = vm.FirstName;
            entity.LastName = vm.LastName;
            entity.Patronymic = vm.Patronymic;
            entity.UseForRequests = vm.UseForRequests;
            entity.JobTitle = vm.JobTitle;
            entity.PhoneNumber = vm.PhoneNumber.CleanPhoneNumber();
            entity.MobilePhoneNumber = vm.MobilePhoneNumber.CleanPhoneNumber()?.LastSymbols(10);
            _db.SaveChanges();
            return entity.Id;
        }

        public SupplierContactPersonVm GetContactPersonsById(Guid personId)
        {
            var entity = _db.SupplierContactPersons
                .IgnoreQueryFilters()
                .FirstOrDefault(q => q.Id == personId);

            return entity?.Adapt<SupplierContactPersonVm>();
        }

        public void DeleteContactPerson(Guid personId)
        {
            var person = _db.SupplierContactPersons.Find(personId);
            if (person == null) return;
            _db.SupplierContactPersons.Remove(person);
            _db.SaveChanges();
        }

        public (Guid SupplierId, Guid ContactPersonId) GetSupplierIdByEmail(Guid ownerId, string email)
        {
            var supplierContactPerson = _db.SupplierContactPersons
                .IgnoreQueryFilters()
                .FirstOrDefault(q =>
                    q.Email.Equals(email) &&
                    q.UseForRequests &&
                    q.Supplier.OwnerId == ownerId);

            return (
                SupplierId: supplierContactPerson?.SupplierId ?? Guid.Empty,
                ContactPersonId: supplierContactPerson?.Id ?? Guid.Empty);
        }

        public string GetSupplierNameByEmail(Guid ownerId, string email)
        {
            var supplierContactPerson = _db.SupplierContactPersons
                .IgnoreQueryFilters()
                .Include(q => q.Supplier)
                .FirstOrDefault(q =>
                    q.Email.Equals(email) &&
                    q.UseForRequests &&
                    q.Supplier.OwnerId == ownerId);

            return supplierContactPerson?.Supplier.Name ?? string.Empty;
        }

        public void Update(SupplierVm model)
        {
            var entity = _db.Suppliers.Find(model.Id);

            if (entity != null)
            {
                if (HasSameSupplierInn(entity.OwnerId, model.Id, model.Inn))
                {
                    throw new SameInnException();
                }

                entity.Name = model.Name;
                entity.OwnershipType = model.OwnershipType;
                entity.Inn = model.Inn;
                entity.ErpCode = model.ErpCode;
                entity.Website = model.Website;
                entity.LegalAddressStreet = model.LegalAddressStreet;
                entity.LegalAddressCity = model.LegalAddressCity;
                entity.LegalAddressCountry = model.LegalAddressCountry;
                entity.ActualAddressStreet = model.ActualAddressStreet;
                entity.ActualAddressCity = model.ActualAddressCity;
                entity.ActualAddressCountry = model.ActualAddressCountry;
                entity.WarehouseAddressStreet = model.WarehouseAddressStreet;
                entity.WarehouseAddressCity = model.WarehouseAddressCity;
                entity.WarehouseAddressCountry = model.WarehouseAddressCountry;
                entity.PriceWithVat = model.PriceWithVat;
                entity.SumWithVat = model.SumWithVat;
                entity.DeliveryTerms = model.DeliveryTerms;
                entity.Note = model.Note;
                entity.OfferCurrency = model.OfferCurrency;
                entity.PaymentDeferredDays = model.PaymentDeferredDays;
                entity.Phone = model.Phone.CleanPhoneNumber();
                entity.SupplierType = model.SupplierType;

                _db.SaveChanges();
            }
        }

        public Guid CreateSupplier(SupplierVm model, Guid ownerId)
        {
            if (HasSameSupplierInn(ownerId, null, model.Inn))
            {
                throw new SameInnException();
            }

            var entry = _db.Suppliers.Add(new Supplier
            {
                Name = model.Name,
                OwnershipType = model.OwnershipType,
                Inn = model.Inn,
                ErpCode = model.ErpCode,
                Website = model.Website,
                LegalAddressStreet = model.LegalAddressStreet,
                LegalAddressCity = model.LegalAddressCity,
                LegalAddressCountry = model.LegalAddressCountry,
                ActualAddressStreet = model.ActualAddressStreet,
                ActualAddressCity = model.ActualAddressCity,
                ActualAddressCountry = model.ActualAddressCountry,
                WarehouseAddressStreet = model.WarehouseAddressStreet,
                WarehouseAddressCity = model.WarehouseAddressCity,
                WarehouseAddressCountry = model.WarehouseAddressCountry,
                PriceWithVat = model.PriceWithVat,
                SumWithVat = model.SumWithVat,
                DeliveryTerms = model.DeliveryTerms,
                Note = model.Note,
                OfferCurrency = model.OfferCurrency,
                PaymentDeferredDays = model.PaymentDeferredDays,
                Phone = model.Phone.CleanPhoneNumber(),
                SupplierType = model.SupplierType,
                PublicId = _counterService.GetSupplierNextId(ownerId)
            });
            _db.SaveChanges();
            return entry.Entity.Id;
        }

        private bool HasSameSupplierInn(Guid ownerId, Guid? exceptSupplierId, long? inn) =>
            inn.HasValue &&
            _db.Suppliers.Any(_ => _.OwnerId == ownerId && _.Id != exceptSupplierId && _.Inn == inn.Value);

        public List<SupplierNomenclatureCategory> GetSupplierNomenclatureCategories(Guid supplierId)
        {
            var supplierCategories = _db.SupplierCategories
                .Include(q => q.PrimaryContactPerson)
                .Include(q => q.SecondaryContactPerson)
                .Where(q => q.PrimaryContactPerson.SupplierId == supplierId || q.SecondaryContactPerson.SupplierId == supplierId)
                .ToList();

            var nalIds = (from n in _db.Nomenclatures
                      join na in _db.NomenclatureAlternatives on n.Id equals na.NomenclatureId
                      join nal in _db.NomenclatureAlternativeLinks on na.Id equals nal.AlternativeId
                      where nal.SupplierId == supplierId && !n.Category.IsDeleted && !n.IsDeleted
                      select n.CategoryId).Distinct().ToList();

            var categoryIds = supplierCategories.Select(q => q.NomenclatureCategoryId).Union(nalIds);

            return categoryIds.Select(ncId =>
            {
                var mapping = supplierCategories.FirstOrDefault(q => q.NomenclatureCategoryId == ncId && (q.PrimaryContactPerson?.SupplierId == supplierId || q.SecondaryContactPerson?.SupplierId == supplierId));

                return new SupplierNomenclatureCategory
                {
                    NomenclatureCategoryId = ncId,
                    NomenclatureCategoryFullName = _categoryService.FullCategoryName(ncId),
                    NomenclatureCategoryPrimaryContactId = mapping?.PrimaryContactPersonId,
                    NomenclatureCategorySecondaryContactId = mapping?.SecondaryContactPersonId,
                    IsDefaultSupplierCategory = mapping?.IsDefault ?? false
                };
            }).ToList();
        }

        public void RemoveSupplierNomenclatureCategoryContacts(Guid supplierId, Guid nomenclatureCategoryId)
        {
            var categories = _db.SupplierCategories.Where(q => q.NomenclatureCategoryId == nomenclatureCategoryId && (q.PrimaryContactPerson.SupplierId == supplierId || q.SecondaryContactPerson.SupplierId == supplierId));
            _db.SupplierCategories.RemoveRange(categories);
            _db.SaveChanges();
        }

        public void RemoveSupplierNomenclatureCategories(Guid supplierId)
        {
            var categories = _db.SupplierCategories.Where(q => q.PrimaryContactPerson.SupplierId == supplierId || q.SecondaryContactPerson.SupplierId == supplierId);
            _db.SupplierCategories.RemoveRange(categories);
            _db.SaveChanges();
        }

        public void SaveSupplierNomenclatureCategoryContacts(Guid supplierId,
            IEnumerable<(Guid NomenclatureCategoryId, Guid? PrimarySupplierContactId, Guid? SecondarySupplierContactId, bool IsDefaultSupplierCategory)> nomenclatureCategories2Contacts)
        {
            if (nomenclatureCategories2Contacts.Any())
            {
                foreach (var mapping in nomenclatureCategories2Contacts)
                {
                    RemoveSupplierNomenclatureCategoryContacts(supplierId, mapping.NomenclatureCategoryId);

                    if (mapping.PrimarySupplierContactId.HasValue || mapping.SecondarySupplierContactId.HasValue)
                    {
                        _db.SupplierCategories.Add(new SupplierCategory
                        {
                            NomenclatureCategoryId = mapping.NomenclatureCategoryId,
                            PrimaryContactPersonId = mapping.PrimarySupplierContactId,
                            SecondaryContactPersonId = mapping.SecondarySupplierContactId,
                            IsDefault = mapping.IsDefaultSupplierCategory
                        });
                    }
                }

                _db.SaveChanges();
            }
        }

        public IEnumerable<SupplierVm> GetByPublicIds(params int[] publicIds)
        {
            if (!publicIds.Any())
            {
                return Enumerable.Empty<SupplierVm>();
            }

            return (from item in _db.Suppliers
                    where publicIds.Contains(item.PublicId)
                    select item).ToList().Select(_ => _.Adapt<SupplierVm>());
        }

        public Dictionary<SupplierVm, IEnumerable<Guid>> GetByCategoryIds(IList<Guid> nomenclatureCategoryIds, IList<Guid> ignoreSupplierIds)
        {
            if (!nomenclatureCategoryIds.Any())
            {
                return new Dictionary<SupplierVm, IEnumerable<Guid>>();
            }

            var suppliersByAlternatives1 = from n in _db.Nomenclatures
                                          join na in _db.NomenclatureAlternatives on n.Id equals na.NomenclatureId
                                          join nal in _db.NomenclatureAlternativeLinks on na.Id equals nal.AlternativeId
                                          join s in _db.Suppliers on nal.SupplierId equals s.Id
                                          join cs in _db.SupplierContactPersons on s.Id equals cs.SupplierId
                                          join c in _db.SupplierCategories on cs.Id equals c.PrimaryContactPersonId
                                          where !n.IsDeleted
                                                && nomenclatureCategoryIds.Contains(n.CategoryId)
                                                && !ignoreSupplierIds.Contains(s.Id) 
                                                && c.PrimaryContactPersonId != null
                                          select new SupplierWCategoryId(s, n.CategoryId, c.IsDefault);

            var suppliersByAlternatives2 = from n in _db.Nomenclatures
                                            join na in _db.NomenclatureAlternatives on n.Id equals na.NomenclatureId
                                            join nal in _db.NomenclatureAlternativeLinks on na.Id equals nal.AlternativeId
                                            join s in _db.Suppliers on nal.SupplierId equals s.Id
                                            join cs in _db.SupplierContactPersons on s.Id equals cs.SupplierId
                                            join c in _db.SupplierCategories on cs.Id equals c.SecondaryContactPersonId
                                            where !n.IsDeleted
                                                  && nomenclatureCategoryIds.Contains(n.CategoryId)
                                                  && !ignoreSupplierIds.Contains(s.Id) 
                                                  && c.SecondaryContactPersonId != null
                                            select new SupplierWCategoryId(s, n.CategoryId, c.IsDefault);
            
            var suppliers = suppliersByAlternatives1.Union(suppliersByAlternatives2).ToList();

            var result = suppliers
                .GroupBy(q => q.Supplier)
                .ToDictionary(
                    g => g.Key.Adapt<SupplierVm>(),
                    g => g.Select(q => q.CategoryId).Distinct().SelectMany(GetCategoryChildIds).Distinct()
                );

            return result;
        }

        private List<Guid> GetCategoryChildIds(Guid parentId)
        {
            var cacheKey = Consts.CacheKeys.SupplierServiceGetCategoryChildIds(parentId);

            if (!_memoryCache.TryGetValue(cacheKey, out List<Guid> ids))
            {
                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    var qry = $";WITH Result AS " +
                              $"(SELECT * FROM [NomenclatureCategories] WHERE Id = '{parentId}' " +
                              $"UNION ALL SELECT t.* FROM [NomenclatureCategories] t " +
                              $"INNER JOIN Result r ON t.ParentID = r.ID) " +
                              $"SELECT Id FROM Result";

                    command.CommandText = qry;
                    _db.Database.OpenConnection();

                    using (var result = command.ExecuteReader())
                    {
                        if (!result.HasRows) return ids;

                        while (result.Read())
                        {
                            ids.Add(result.GetGuid(0));
                        }
                    }
                }

                _memoryCache.Set(cacheKey, ids, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(10) });
            }

            return ids;
        }

        class SupplierWCategoryId
        {
            public Supplier Supplier { get; }
            public Guid CategoryId { get; }
            public bool IsDefault { get; }

            public SupplierWCategoryId(Supplier supplier, Guid categoryId, bool isDefault)
            {
                Supplier = supplier;
                CategoryId = categoryId;
                IsDefault = isDefault;
            }
        }
    }
}

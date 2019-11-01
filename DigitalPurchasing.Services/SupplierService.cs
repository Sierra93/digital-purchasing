using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using DigitalPurchasing.Services.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace DigitalPurchasing.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext _db;
        private const StringComparison StrComparison = StringComparison.InvariantCultureIgnoreCase;
        private readonly INomenclatureCategoryService _categoryService;
        private readonly ICounterService _counterService;

        public SupplierService(
            ApplicationDbContext db,
            INomenclatureCategoryService categoryService,
            ICounterService counterService)
        {
            _db = db;
            _categoryService = categoryService;
            _counterService = counterService;
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
            var supplier2NomCategories = (from s in qry
                                          select new
                                          {
                                              supplierId = s.Id,
                                              defaultCategoryId = s.CategoryId
                                          }).ToList();

            foreach (var item in result)
            {                
                var supplierCategoryIds = supplier2altNomCategories
                    .Where(_ => _.supplierId == item.Id)
                    .Select(_ => _.altCategoryId)
                    .ToList();
                Guid? defaultCategoryId = supplier2NomCategories
                    .FirstOrDefault(_ => _.supplierId == item.Id)?.defaultCategoryId;
                if (defaultCategoryId.HasValue)
                {
                    supplierCategoryIds.Add(defaultCategoryId.Value);
                }

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
                entity.CategoryId = model.CategoryId;

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
                CategoryId = model.CategoryId,
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
            var qry = from n in _db.Nomenclatures.Where(q => !q.IsDeleted)
                      join na in _db.NomenclatureAlternatives on n.Id equals na.NomenclatureId
                      join nal in _db.NomenclatureAlternativeLinks on na.Id equals nal.AlternativeId
                      where nal.SupplierId == supplierId &&
                            !n.Category.IsDeleted
                      select n.CategoryId;

            var categoryIds = qry.Distinct().ToList();

            var defaultCategoryId = _db.Suppliers
                .Where(_ => _.Id == supplierId)
                .Select(_ => _.CategoryId)
                .FirstOrDefault();

            if (defaultCategoryId.HasValue && !categoryIds.Contains(defaultCategoryId.Value))
            {
                categoryIds.Add(defaultCategoryId.Value);
            }

            return categoryIds.Select(ncId =>
            {
                var mapping = _db.SupplierCategories.Where(_ =>
                    _.NomenclatureCategoryId == ncId &&
                    (_.PrimaryContactPerson.SupplierId == supplierId || _.SecondaryContactPerson.SupplierId == supplierId)).FirstOrDefault();
                return new SupplierNomenclatureCategory()
                {
                    NomenclatureCategoryId = ncId,
                    NomenclatureCategoryFullName = _categoryService.FullCategoryName(ncId),
                    NomenclatureCategoryPrimaryContactId = mapping?.PrimaryContactPersonId,
                    NomenclatureCategorySecondaryContactId = mapping?.SecondaryContactPersonId,
                    IsDefaultSupplierCategory = defaultCategoryId == ncId
                };
            }).ToList();
        }

        public void RemoveSupplierNomenclatureCategoryContacts(Guid supplierId, Guid nomenclatureCategoryId)
        {
            _db.SupplierCategories.RemoveRange(
                _db.SupplierCategories.Where(_ => _.NomenclatureCategoryId == nomenclatureCategoryId &&
                    (_.PrimaryContactPerson.SupplierId == supplierId || _.SecondaryContactPerson.SupplierId == supplierId)));
            _db.SaveChanges();
        }

        public void SaveSupplierNomenclatureCategoryContacts(Guid supplierId,
            IEnumerable<(Guid nomenclatureCategoryId, Guid? primarySupplierContactId, Guid? secondarySupplierContactId)> nomenclatureCategories2Contacts)
        {
            if (nomenclatureCategories2Contacts.Any())
            {
                foreach (var mapping in nomenclatureCategories2Contacts)
                {
                    RemoveSupplierNomenclatureCategoryContacts(supplierId, mapping.nomenclatureCategoryId);

                    if (mapping.primarySupplierContactId.HasValue ||
                        mapping.secondarySupplierContactId.HasValue)
                    {
                        _db.SupplierCategories.Add(new SupplierCategory()
                        {
                            NomenclatureCategoryId = mapping.nomenclatureCategoryId,
                            PrimaryContactPersonId = mapping.primarySupplierContactId,
                            SecondaryContactPersonId = mapping.secondarySupplierContactId
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

            var suppliersByAlternatives = from n in _db.Nomenclatures.Where(q => !q.IsDeleted)
                                          join na in _db.NomenclatureAlternatives on n.Id equals na.NomenclatureId
                                          join nal in _db.NomenclatureAlternativeLinks on na.Id equals nal.AlternativeId
                                          join s in _db.Suppliers on nal.SupplierId equals s.Id
                                          where nomenclatureCategoryIds.Contains(n.CategoryId) && !ignoreSupplierIds.Contains(s.Id)
                                          select new SupplierWCategoryId(s, n.CategoryId, false);

            var suppliersByMainCategories = from s in _db.Suppliers
                                            where s.CategoryId.HasValue && nomenclatureCategoryIds.Contains(s.CategoryId.Value) && !ignoreSupplierIds.Contains(s.Id)
                                            select new SupplierWCategoryId(s, s.CategoryId.Value, true);

            var suppliers = suppliersByAlternatives
                .Union(suppliersByMainCategories)
                .OrderBy(s => s.Supplier.Name)
                .ToList();

            var suppliersIds = suppliers.Select(q => q.Supplier.Id).Distinct().ToList();

            var categories = _db.SupplierCategories
                .Include(q => q.PrimaryContactPerson)
                .Include(q => q.SecondaryContactPerson)
                .Where(q => q.PrimaryContactPersonId.HasValue && suppliersIds.Contains(q.PrimaryContactPerson.SupplierId)
                            || q.SecondaryContactPersonId.HasValue && suppliersIds.Contains(q.SecondaryContactPerson.SupplierId))
                .ToList();

            var resultSuppliers = new List<SupplierWCategoryId>();

            foreach (var supplier in suppliers)
            {
                var haveContacts = categories
                    .Any(q => (q.PrimaryContactPersonId.HasValue && q.PrimaryContactPerson.SupplierId == supplier.Supplier.Id && q.NomenclatureCategoryId == supplier.CategoryId)
                           || (q.SecondaryContactPersonId.HasValue && q.SecondaryContactPerson.SupplierId == supplier.Supplier.Id && q.NomenclatureCategoryId == supplier.CategoryId));

                if (haveContacts)
                {
                    resultSuppliers.Add(supplier);
                }
            }

            var result = resultSuppliers
                .GroupBy(q => q.Supplier)
                .ToDictionary(
                    g => g.Key.Adapt<SupplierVm>(),
                    g => g.Select(q => q.CategoryId).Distinct()
                );

            return result;
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

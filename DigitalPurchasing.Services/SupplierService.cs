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

namespace DigitalPurchasing.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext _db;
        private const StringComparison StrComparison = StringComparison.InvariantCultureIgnoreCase;
        private readonly INomenclatureCategoryService _categoryService;

        public SupplierService(
            ApplicationDbContext db,
            INomenclatureCategoryService categoryService)
        {
            _db = db;
            _categoryService = categoryService;
        }

        public SupplierAutocomplete Autocomplete(AutocompleteBaseOptions options)
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
                result.Items = entities.ToList();
            }

            return result;
        }

        public Guid CreateSupplier(string name)
        {
            var entry = _db.Suppliers.Add(new Supplier { Name = name });
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

            var supplier2nomCategories = (from s in qry
                                          join nal in _db.NomenclatureAlternativeLinks on s.Id equals nal.SupplierId
                                          join na in _db.NomenclatureAlternatives on nal.AlternativeId equals na.Id
                                          join n in _db.Nomenclatures on na.NomenclatureId equals n.Id
                                          where !n.IsDeleted
                                          group new
                                          {
                                              supplierId = s.Id,
                                              n.CategoryId
                                          } by new { s.Id, n.CategoryId } into g
                                          select g.FirstOrDefault()).ToList();

            foreach (var item in result)
            {
                var supplierCategoryIds = supplier2nomCategories
                    .Where(_ => _.supplierId == item.Id)
                    .Select(_ => _.CategoryId);
                item.MainCategoriesCsv = string.Join(", ", supplierCategoryIds.Select(cId => _categoryService.GetParentCategory(cId).Name));
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

        public List<SupplierContactPersonVm> GetContactPersonsBySupplier(Guid supplierId)
            => _db.SupplierContactPersons
                .Where(q => q.SupplierId == supplierId)
                .ProjectToType<SupplierContactPersonVm>()
                .ToList();

        public Guid AddContactPerson(SupplierContactPersonVm vm)
        {
            var contactPerson = vm.Adapt<SupplierContactPerson>();
            contactPerson.PhoneNumber = contactPerson.PhoneNumber.CleanPhoneNumber();
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
            _db.SaveChanges();
            return entity.Id;
        }

        public SupplierContactPersonVm GetContactPersonsById(Guid personId)
        {
            var entity = _db.SupplierContactPersons.Find(personId);
            return entity?.Adapt<SupplierContactPersonVm>();
        }

        public void DeleteContactPerson(Guid personId)
        {
            var person = _db.SupplierContactPersons.Find(personId);
            if (person == null) return;
            _db.SupplierContactPersons.Remove(person);
            _db.SaveChanges();
        }

        public SupplierContactPersonVm GetContactPersonBySupplier(Guid supplierId)
        {
            var supplierContactPerson = _db.SupplierContactPersons
                .FirstOrDefault(q => q.SupplierId == supplierId && q.UseForRequests);

            return supplierContactPerson?.Adapt<SupplierContactPersonVm>();
        }

        public Guid GetSupplierByEmail(string email)
        {
            var supplierContactPerson = _db.SupplierContactPersons.IgnoreQueryFilters()
                .FirstOrDefault(q => q.Email.Equals(email) && q.UseForRequests);

            return supplierContactPerson?.SupplierId ?? Guid.Empty;
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
                SumWithVat = model.SumWithVat
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

            return qry.Distinct().ToList().Select(ncId =>
            {
                var mapping = _db.SupplierCategories.Where(_ =>
                    _.NomenclatureCategoryId == ncId &&
                    (_.PrimaryContactPerson.SupplierId == supplierId || _.SecondaryContactPerson.SupplierId == supplierId)).FirstOrDefault();
                return new SupplierNomenclatureCategory()
                {
                    NomenclatureCategoryId = ncId,
                    NomenclatureCategoryFullName = _categoryService.FullCategoryName(ncId),
                    NomenclatureCategoryPrimaryContactId = mapping?.PrimaryContactPersonId,
                    NomenclatureCategorySecondaryContactId = mapping?.SecondaryContactPersonId
                };
            }).ToList();
        }

        public void SaveSupplierNomenclatureCategoryContacts(Guid supplierId,
            IEnumerable<(Guid nomenclatureCategoryId, Guid? primarySupplierContactId, Guid? secondarySupplierContactId)> nomenclatureCategories2Contacts)
        {
            foreach (var mapping in nomenclatureCategories2Contacts)
            {
                _db.SupplierCategories.RemoveRange(
                    _db.SupplierCategories.Where(_ => _.NomenclatureCategoryId == mapping.nomenclatureCategoryId));

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
}

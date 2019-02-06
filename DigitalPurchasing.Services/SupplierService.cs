using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Core.Extensions;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;

namespace DigitalPurchasing.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext _db;
        private const StringComparison StrComparison = StringComparison.InvariantCultureIgnoreCase;

        public SupplierService(ApplicationDbContext db) => _db = db;

        public SupplierAutocomplete Autocomplete(AutocompleteBaseOptions options)
        {
            var entities = _db.Suppliers
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

        public SupplierIndexData GetData(int page, int perPage, string sortField, bool sortAsc)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }
            
            var qry = _db.Suppliers.AsQueryable();//.Where(q => !q.IsDeleted);
            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc?"":" DESC")}");
            var result = orderedResults.Skip((page-1)*perPage).Take(perPage).ProjectToType<SupplierIndexDataItem>().ToList();

            return new SupplierIndexData
            {
                Total = total,
                Data = result
            };
        }

        public SupplierVm GetById(Guid id) => _db.Suppliers.Find(id)?.Adapt<SupplierVm>();

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
    }
}

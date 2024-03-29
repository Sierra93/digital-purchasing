using System;
using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using System.Linq.Dynamic.Core;
using DigitalPurchasing.Services.Exceptions;
using System.Collections.Generic;

namespace DigitalPurchasing.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _db;
        private const StringComparison StrComparison = StringComparison.InvariantCultureIgnoreCase;
        private readonly ICounterService _counterService;

        public CustomerService(ApplicationDbContext db, ICounterService counterService)
        {
            _db = db;
            _counterService = counterService;
        }

        public CustomerAutocomplete Autocomplete(AutocompleteBaseOptions options)
        {
            var entities = _db.Customers
                .Where(q => !string.IsNullOrEmpty(q.Name))
                .Where(q => q.Name.Equals(options.Query, StrComparison) || q.Name.Contains(options.Query, StrComparison))
                .OrderBy(q => q.Name)
                .Select(q => new CustomerAutocomplete.Customer { Name = q.Name, Id = q.Id })
                .ToList();

            var result = new CustomerAutocomplete();

            if (entities.Any())
            {
                result.Items = entities.ToList();
            }

            return result;
        }

        public Guid CreateCustomer(string name, Guid ownerId)
        {
            var entry = _db.Customers.Add(new Customer
            {
                Name = name,
                PublicId = _counterService.GetCustomerNextId(ownerId)
            });
            _db.SaveChanges();
            return entry.Entity.Id;
        }

        public string GetNameById(Guid id) => _db.Customers.Find(id).Name;

        public CustomerVm GetById(Guid id) => _db.Customers.Find(id)?.Adapt<CustomerVm>();

        public CustomerIndexData GetData(int page, int perPage, string sortField, bool sortAsc, string search)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = "Name";
            }

            var qry = _db.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                qry = qry.Where(q =>
                    !string.IsNullOrEmpty(q.Name) && q.Name.Contains(search));
            }

            var total = qry.Count();
            var orderedResults = qry.OrderBy($"{sortField}{(sortAsc ? "" : " DESC")}");
            var result = orderedResults.Skip((page - 1) * perPage).Take(perPage).ProjectToType<CustomerIndexDataItem>().ToList();

            return new CustomerIndexData
            {
                Total = total,
                Data = result
            };
        }

        public void Update(CustomerVm model)
        {
            var entity = _db.Customers.Find(model.Id);
            if (entity != null)
            {
                entity.Name = model.Name;
                _db.SaveChanges();
            }
        }

        public void Delete(Guid id)
        {
            var entity = _db.Customers.Find(id);
            if (entity != null)
            {
                if (IsCustomerInUse(id))
                {
                    throw new CustomerInUseException();
                }

                _db.Customers.Remove(entity);
                _db.SaveChanges();
            }            
        }

        public bool IsCustomerInUse(Guid id) => _db.PurchaseRequests.Any(pr => pr.CustomerId == id);

        public IEnumerable<CustomerVm> GetByPublicIds(params int[] publicIds)
        {
            if (!publicIds.Any())
            {
                return Enumerable.Empty<CustomerVm>();
            }

            return (from item in _db.Customers
                    where publicIds.Contains(item.PublicId)
                    select item).ToList().Select(_ => _.Adapt<CustomerVm>());
        }
    }
}

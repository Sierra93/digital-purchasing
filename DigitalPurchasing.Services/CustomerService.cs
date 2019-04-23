using System;
using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using System.Linq.Dynamic.Core;

namespace DigitalPurchasing.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _db;
        private const StringComparison StrComparison = StringComparison.InvariantCultureIgnoreCase;

        public CustomerService(ApplicationDbContext db) => _db = db;

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

        public Guid CreateCustomer(string name)
        {
            var entry = _db.Customers.Add(new Customer { Name = name });
            _db.SaveChanges();
            return entry.Entity.Id;
        }

        public string GetNameById(Guid id) => _db.Customers.Find(id).Name;

        public CustomerVm GetById(Guid id) => _db.Suppliers.Find(id)?.Adapt<CustomerVm>();

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
    }
}

using System;
using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;

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
    }
}

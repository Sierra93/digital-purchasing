using System;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;

namespace DigitalPurchasing.Services
{
    public interface ICompanyService
    {
        CompanyResult Create(string name);
    }

    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _db;

        public CompanyService(ApplicationDbContext db) => _db = db;

        public CompanyResult Create(string name)
        {
            var entry = _db.Companies.Add(new Company {Name = name});
            _db.SaveChanges();
            return entry.Entity.Adapt<CompanyResult>();;
        }
    }

    public class CompanyResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}

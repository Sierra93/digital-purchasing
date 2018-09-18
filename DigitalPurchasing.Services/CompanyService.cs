using System;
using System.Linq;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _db;

        public CompanyService(ApplicationDbContext db) => _db = db;

        public CompanyResponse Create(string name)
        {
            var entry = _db.Companies.Add(new Company {Name = name});
            _db.SaveChanges();
            var response = entry.Entity.Adapt<CompanyResponse>();
            response.IsOwner = true;
            return response;
        }

        public CompanyResponse GetByUser(Guid userId)
        {
            var user = _db.Users.Include(q => q.Company).FirstOrDefault(q => q.Id == userId);
            if (user == null) return null;
            var response = user.Company.Adapt<CompanyResponse>();
            response.IsOwner = true;
            return response;
        }

        public void UpdateName(Guid userId, string newName)
        {
            var user = _db.Users.Include(q => q.Company).FirstOrDefault(q => q.Id == userId);
            if (user == null) return;
            user.Company.Name = newName;
            _db.SaveChanges();
        }
    }
}

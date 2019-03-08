using System;
using System.Linq;
using System.Security.Claims;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models;
using DigitalPurchasing.Models.Identity;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _db;

        private readonly UserManager<User> _userManager;

        public CompanyService(
            ApplicationDbContext db,
            UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public CompanyResponse Create(string name)
        {
            var entry = _db.Companies.Add(new Company {Name = name });
            _db.SaveChanges();
            var response = entry.Entity.Adapt<CompanyResponse>();
            response.IsOwner = true;
            return response;
        }

        public void AssignOwner(Guid companyId, Guid userId)
        {
            var user = _userManager.FindByIdAsync(userId.ToString("N")).Result;
            var result = _userManager.AddToRoleAsync(user, Consts.Roles.CompanyOwner).Result;
            if (!result.Succeeded)
            {
                throw new Exception();
            }
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

        public string GetContactEmailByOwner(Guid ownerId)
        {
            var user = _db.Users.FirstOrDefault(q => q.CompanyId == ownerId);
            if (user != null && user.EmailConfirmed) return user.Email;
            return null;
        }
    }
}

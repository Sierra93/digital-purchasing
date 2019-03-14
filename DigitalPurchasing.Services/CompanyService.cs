using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public CompanyDto Create(string name)
        {
            var entry = _db.Companies.Add(new Company {Name = name });
            _db.SaveChanges();
            var response = entry.Entity.Adapt<CompanyDto>();
            return response;
        }

        public async Task AssignOwner(Guid companyId, Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString("N"));
            var result = await _userManager.AddToRoleAsync(user, Consts.Roles.CompanyOwner);
            if (!result.Succeeded)
            {
                throw new Exception();
            }
        }

        public CompanyDto GetByUser(Guid userId)
        {
            var user = _db.Users.Include(q => q.Company).FirstOrDefault(q => q.Id == userId);
            var response = user?.Company.Adapt<CompanyDto>();
            return response;
        }

        public async Task<CompanyDto> GetByInvitationCode(string code)
        {
            var company = await _db.Companies.FirstOrDefaultAsync(q => q.InvitationCode == code);
            return company?.Adapt<CompanyDto>();
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

        public async Task<bool> IsValidInvitationCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return false;
            return await _db.Companies.AnyAsync(q => q.InvitationCode == code);
        }

        public async Task<string> GetInvitationCode(Guid companyId)
        {
            var company =  await _db.Companies.FindAsync(companyId);
            return company?.InvitationCode;
        }

        public async Task<bool> HaveOwner(Guid companyId)
        {
            var company =  await _db.Companies.FindAsync(companyId);
            if (company == null)
                throw new Exception();

            var owners = await _userManager.GetUsersInRoleAsync(Consts.Roles.CompanyOwner);
            return owners.Any(q => q.CompanyId == companyId);
        }

        public async Task<List<CompanyUserDto>> GetCompanyUsers(Guid companyId)
        {
            var users = await _db.Users.Where(q => q.CompanyId == companyId).ToListAsync();
            if (users.Any())
            {
                var companyUsers = users.Adapt<List<CompanyUserDto>>();

                foreach (var companyUser in companyUsers)
                {
                    companyUser.IsCompanyOwner = await _userManager.IsInRoleAsync(
                        users.Find(q => q.Id == companyUser.Id),
                        Consts.Roles.CompanyOwner);
                }

                return companyUsers;
            }

            return new List<CompanyUserDto>();
        }

        public async Task<int> Count() => await _db.Companies.CountAsync();
        public async Task<List<CompanyDto>> GetAll()
        {
            var companies = await _db.Companies.ToListAsync();
            return companies.Adapt<List<CompanyDto>>();
        }
    }
}

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
        private readonly IUomService _uomService;

        public CompanyService(
            ApplicationDbContext db,
            UserManager<User> userManager,
            IUomService uomService)
        {
            _db = db;
            _userManager = userManager;
            _uomService = uomService;
        }

        public async Task<CompanyDto> Create(string name)
        {
            var entry = await _db.Companies.AddAsync(new Company
            {
                Name = name,
                InvitationCode = Guid.NewGuid().ToString("N")
            });
            _db.SaveChanges();
            await SeedCompanyData(entry.Entity.Id);
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

        public async Task<CompanyDto> GetById(Guid companyId)
        {
            var company = await _db.Companies.FirstOrDefaultAsync(q => q.Id == companyId);
            return company?.Adapt<CompanyDto>();
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
            if (string.IsNullOrEmpty(user.Company.InvitationCode))
            {
                user.Company.InvitationCode = Guid.NewGuid().ToString("N");
            }
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

        public bool IsValidOwnerId(Guid ownerId)
        {
            var company = _db.Companies.FirstOrDefault(q => q.Id == ownerId);
            return company != null;
        }

        public async Task<bool> IsCompanyOwner(Guid companyId, Guid userId)
            => await _db.UserRoles
                .Include(q => q.Role)
                .Include(q => q.User)
                .AnyAsync(q => q.Role.Name == Consts.Roles.CompanyOwner
                               && q.UserId == userId
                               && q.User.CompanyId == companyId);

        public async Task<bool> UserCanDeleteSupplierOffers(Guid companyId, Guid userId)
        {
            if (await IsCompanyOwner(companyId, userId)) return true;

            var company = await GetById(companyId);
            if (company.IsSODeleteEnabled) return true;
            
            return false;
        }

        public async Task Update(CompanyDto company)
        {
            var entity = await _db.Companies.FirstOrDefaultAsync(q => q.Id == company.Id);
            entity.Name = company.Name;
            entity.IsSODeleteEnabled = company.IsSODeleteEnabled;
            await _db.SaveChangesAsync();
        }

        private async Task SeedCompanyData(Guid companyId)
        {
            var psc1 = await _uomService.Create(companyId, "шт", 1);
            var psc1000 = await _uomService.Create(companyId, "тыс. шт", 1000);

            var kilogram = await _uomService.Create(companyId, "кг");
            var ton = await _uomService.Create(companyId, "т");

            var hour = await _uomService.Create(companyId, "час");
            var month = await _uomService.Create(companyId, "мес");

            var durability = await _uomService.Create(companyId, "стойкость");
            var resource = await _uomService.Create(companyId, "ресурс");

            var packaging = await _uomService.Create(companyId, "упаковка");

            _uomService.SaveConversionRate(companyId, psc1000.Id, psc1.Id, null, 1000, 0);
            _uomService.SaveConversionRate(companyId, ton.Id, kilogram.Id, null, 1000, 0);

            await _uomService.SetPackagingUom(companyId, packaging.Id);
            await _uomService.SetMassUom(companyId, kilogram.Id);
            await _uomService.SetResourceUom(companyId, resource.Id);
            await _uomService.SetResourceBatchUom(companyId, month.Id);
        }
    }
}

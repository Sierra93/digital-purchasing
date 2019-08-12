using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using DigitalPurchasing.Models.Identity;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public UserService(
            ApplicationDbContext db,
            RoleManager<Role> roleManager,
            UserManager<User> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public UserInfoDto GetUserInfo(Guid userId)
        {
            var user = _db.Users.Include(q => q.Company).FirstOrDefault(q => q.Id == userId);
            if (user == null) return null;

            return new UserInfoDto
            {
                Company = user.Company.Name,
                LastName = user.LastName,
                FirstName = user.FirstName,
                Patronymic = user.Patronymic,
                JobTitle = user.JobTitle,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<UserDto> GetCompanyOwner(Guid companyId)
        {
            var companyOwnerRole = await _roleManager.FindByNameAsync(Consts.Roles.CompanyOwner);
            var companyOwner = await _db.UserRoles
                .Include(q => q.User)
                .FirstOrDefaultAsync(q =>
                    q.RoleId == companyOwnerRole.Id &&
                    q.User.CompanyId == companyId);
            return companyOwner?.User.Adapt<UserDto>();
        }

        public async Task<int> TotalCount() => await _db.Users.CountAsync();
        public async Task<int> TotalCountByCompany(Guid companyId) => await _db.Users.CountAsync(q => q.CompanyId == companyId);

        public async Task<int> ConfirmedEmailCount() => await _db.Users.CountAsync(q => q.EmailConfirmed);
    }
}

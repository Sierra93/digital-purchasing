using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalPurchasing.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;

        public UserService(ApplicationDbContext db) => _db = db;

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
                JobTitle = user.JobTitle
            };
        }
    }
}

using System;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IUserService
    {
        UserInfoDto GetUserInfo(Guid userId);
    }

    public class UserInfoDto
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        
        public string Company { get; set; }
        public string JobTitle { get; set; }
    }
}

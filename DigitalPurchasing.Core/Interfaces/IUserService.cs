using System;
using System.Threading.Tasks;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IUserService
    {
        UserInfoDto GetUserInfo(Guid userId);
        Task<UserDto> GetCompanyOwner(Guid companyId);
        Task<int> TotalCount();
        Task<int> TotalCountByCompany(Guid companyId);
        Task<int> ConfirmedEmailCount();
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string JobTitle { get; set; }
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

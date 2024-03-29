using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanyDto> Create(string name);
        CompanyDto GetByUser(Guid userId);
        Task<CompanyDto> GetById(Guid companyId);
        Task<CompanyDto> GetByInvitationCode(string code);
        void UpdateName(Guid userId, string newName);
        Task<bool> HaveOwner(Guid companyId);
        Task AssignOwner(Guid companyId, Guid userId);
        string GetContactEmailByOwner(Guid ownerId);
        Task<bool> IsValidInvitationCode(string code);
        Task<string> GetInvitationCode(Guid companyId);
        Task<List<CompanyUserDto>> GetCompanyUsers(Guid companyId);
        Task<int> Count();
        Task<List<CompanyDto>> GetAll();
        bool IsValidOwnerId(Guid ownerId);
        Task<bool> IsCompanyOwner(Guid companyId, Guid userId);
        Task<bool> UserCanDeleteSupplierOffers(Guid companyId, Guid userId);
        Task Update(CompanyDto company);
    }

    public class CompanyDto
    {
        public bool IsSODeleteEnabled { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class CompanyUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string PhoneNumber { get; set; }
        public string JobTitle { get; set; }
        public bool IsCompanyOwner { get; set;}
    }
}

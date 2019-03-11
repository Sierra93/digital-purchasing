using System;
using System.Threading.Tasks;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICompanyService
    {
        CompanyDto Create(string name);
        CompanyDto GetByUser(Guid userId);
        Task<CompanyDto> GetByInvitationCode(string code);
        void UpdateName(Guid userId, string newName);
        Task<bool> HaveOwner(Guid companyId);
        Task AssignOwner(Guid companyId, Guid userId);
        string GetContactEmailByOwner(Guid ownerId);
        Task<bool> IsValidInvitationCode(string code);
        Task<string> GetInvitationCode(Guid companyId);
    }

    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}

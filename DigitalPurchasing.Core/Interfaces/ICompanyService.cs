using System;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICompanyService
    {
        CompanyResponse Create(string name);
        CompanyResponse GetByUser(Guid userId);
        void UpdateName(Guid userId, string newName);
        string GetContactEmailByOwner(Guid ownerId);
    }

    public class CompanyResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsOwner { get; set; }
    }
}

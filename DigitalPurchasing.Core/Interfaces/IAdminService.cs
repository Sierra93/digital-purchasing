using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardDto> GetDashboard();
        Task<IEnumerable<AdminCompanyDto>> GetCompanies();
    }

    public class AdminDashboardDto
    {
        public int CompaniesCount { get; set; }
        public int UsersCount { get; set; }
        public int ConfirmedUsersCount { get; set; }
    }

    public class AdminCompanyDto
    {
        public class OwnerData
        {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string Patronymic { get; set; }
            public string JobTitle { get; set; }
            public bool EmailConfirmed { get; set; }

            public override string ToString()
            {
                var fullName = $"{LastName ?? ""} {FirstName ?? ""} {Patronymic ?? ""}".Trim();
                if (!string.IsNullOrEmpty(fullName))
                {
                    if (!string.IsNullOrEmpty(JobTitle))
                    {
                        return $"{GetFullEmail()} ({fullName} / {JobTitle})";
                    }
                    return $"{GetFullEmail()} ({fullName})";
                }

                return $"{GetFullEmail()}";
            }

            private string GetFullEmail() => EmailConfirmed ? Email : $"{Email} неподтвержден";
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }

        public OwnerData Owner { get; set; } = new OwnerData();
        
        public int PRCount { get; set; }
        public int QRCount { get; set; }
        public int CLCount { get; set; }

        public int UsersCount { get; set; }
    }
}

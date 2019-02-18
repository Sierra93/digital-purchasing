using System;
using Microsoft.AspNetCore.Identity;

namespace DigitalPurchasing.Models.Identity
{
    public class User : IdentityUser<Guid>
    {
        public User() => Id = Guid.NewGuid();

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }

        public string JobTitle { get; set; }
    }
}

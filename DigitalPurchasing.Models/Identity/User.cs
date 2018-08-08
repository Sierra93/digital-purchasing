using System;
using Microsoft.AspNetCore.Identity;

namespace DigitalPurchasing.Models.Identity
{
    public class User : IdentityUser<Guid>
    {
        public User()
        {
            Id = Guid.NewGuid();
        } 

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
    }
}

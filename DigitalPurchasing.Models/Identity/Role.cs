using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DigitalPurchasing.Models.Identity
{
    public class Role : IdentityRole<Guid>
    {
        public ICollection<UserRole> UserRoles { get; set; }
    }
}

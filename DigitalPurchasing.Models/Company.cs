using System;
using System.Collections.Generic;
using DigitalPurchasing.Models.Identity;

namespace DigitalPurchasing.Models
{
    public class Company : BaseModel<Guid>
    {
        public Company() => Id = Guid.NewGuid();

        public string Name { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}

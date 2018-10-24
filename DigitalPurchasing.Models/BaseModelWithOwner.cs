using System;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Models
{
    public abstract class BaseModelWithOwner : BaseModel, IHaveOwner
    {
        public Guid OwnerId { get; set; } 
        public Company Owner { get; set; }
    }
}

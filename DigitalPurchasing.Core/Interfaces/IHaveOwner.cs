using System;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IHaveOwner
    {
        Guid OwnerId { get; set; }
    }
}

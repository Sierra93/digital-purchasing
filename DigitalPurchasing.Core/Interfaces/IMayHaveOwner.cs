using System;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IMayHaveOwner
    {
        Guid? OwnerId { get; set; }
    }
}

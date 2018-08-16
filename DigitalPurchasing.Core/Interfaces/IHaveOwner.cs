using System;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface IHaveOwner
    {
        Guid OwnerId { get; set; }
    }

    public interface IMayHaveOwner
    {
        Guid? OwnerId { get; set; }
    }
}

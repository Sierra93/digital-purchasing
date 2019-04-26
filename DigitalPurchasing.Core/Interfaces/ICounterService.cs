using System;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ICounterService
    {
        int GetQRNextId(Guid? ownerId = null);
        int GetPRNextId(Guid? ownerId = null);
        int GetCLNextId(Guid? ownerId = null);
        int GetSONextId(Guid? ownerId = null);
        int GetCustomerNextId(Guid ownerId);
    }
}

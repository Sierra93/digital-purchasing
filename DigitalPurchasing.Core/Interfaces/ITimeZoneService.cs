using System;

namespace DigitalPurchasing.Core.Interfaces
{
    public interface ITimeZoneService
    {
        string GetUserTimeZoneId(Guid userId);
    }
}

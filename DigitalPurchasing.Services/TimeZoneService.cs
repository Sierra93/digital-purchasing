using System;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;

namespace DigitalPurchasing.Services
{
    public class TimeZoneService : ITimeZoneService
    {
        public string GetUserTimeZoneId(Guid userId) => "Europe/Moscow";
    }
}

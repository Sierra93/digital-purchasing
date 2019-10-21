using System;

namespace DigitalPurchasing.Core
{
    public static class CustomTimeZoneConverter
    {
        public static TimeZoneInfo GetTimeZoneInfo(string timeZoneId)
            => TimeZoneConverter.TZConvert.GetTimeZoneInfo(timeZoneId);
    }
}

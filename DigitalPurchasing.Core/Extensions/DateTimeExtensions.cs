using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToRussianStandardTime(this DateTime dateTime)
            => TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));

    }
}

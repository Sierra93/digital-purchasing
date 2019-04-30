using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Core.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal ToExcelRound(this decimal value, int decimals) => Round(value, decimals);

        private static decimal Round(decimal value, int decimals)
        {
            if (decimals < 0)
            {
                var factor = (decimal)Math.Pow(10, -decimals);
                return Round(value / factor, 0) * factor;
            }
            return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        }
    }
}

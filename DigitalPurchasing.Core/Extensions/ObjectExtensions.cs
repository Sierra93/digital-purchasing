using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalPurchasing.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
    }
}

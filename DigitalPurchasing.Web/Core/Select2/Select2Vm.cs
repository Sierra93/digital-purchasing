using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPurchasing.Web.Core.Select2
{
    public class Select2Get
    {
        [FromQuery(Name = "term")] public  string Term { get; set; }
        [FromQuery(Name = "_type")] public  string Type { get; set; }
        [FromQuery(Name = "q")] public string Q { get; set; }
        [FromQuery(Name = "page")] public int? Page { get; set; } = 1;
    }
}

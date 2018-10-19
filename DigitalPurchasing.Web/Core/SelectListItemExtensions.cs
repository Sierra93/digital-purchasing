using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DigitalPurchasing.Web.Core
{
    public static class SelectListItemExtensions
    {
        public static List<SelectListItem> AddEmpty(this List<SelectListItem> items)
        {
            items.Insert(0, new SelectListItem("-- Не выбрано --", string.Empty));
            return items;
        }
    }
}

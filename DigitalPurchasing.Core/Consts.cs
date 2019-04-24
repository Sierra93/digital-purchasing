using System;
using DigitalPurchasing.Core.Extensions;

namespace DigitalPurchasing.Core
{
    public static class Consts
    {
        public static class Settings
        {
            public static string AppPath = "App";
        }

        public static class Format
        {
            public static string Qty = "0.####";
            public static string Money2 = "N2";
            public static string Money1 = "N1";
        }

        public static class CacheKeys
        {
            public static string UomAutocomplete(Guid ownerId, string qry)
                => $"uom_ac_{ownerId:N}_{qry.ToMD5()}";

            public static string NomenclatureAutocomplete(Guid ownerId)
                => $"nom_ac_{ownerId:N}";

            public static string NomenclatureAutocompleteSearchInAlts(Guid ownerId, Guid clientId)
                => $"nom_ac_alt_{ownerId:N}_{clientId:N}";
        }

        public static class Roles
        {
            public static string Admin = "Admin";
            public static string CompanyOwner = "CompanyOwner";
        }
    }
}

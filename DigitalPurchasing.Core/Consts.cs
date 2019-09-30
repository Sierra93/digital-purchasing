using System;
using DigitalPurchasing.Core.Enums;
using DigitalPurchasing.Core.Extensions;

namespace DigitalPurchasing.Core
{
    public static class Consts
    {
        public static class Settings
        {
            public static string AppPath = "App";
            public static decimal PRDiscountPercentage = 3m;
            public static decimal QuotationRequestResponseHours = 1m;
            public static decimal PriceReductionResponseHours = 0.5m;
            public static decimal AutoCloseCLHours = 2m;
            public static int RoundsCount = 1;
            public static SendPriceReductionTo SendPriceReductionTo = SendPriceReductionTo.MinPrice;
        }

        public static class Format
        {
            public static string Qty = "0.####";
            public static string Money3 = "N3";
            public static string Money2 = "N2";
            public static string Money1 = "N1";
        }

        public static class CacheKeys
        {
            public static string UomAutocomplete(Guid ownerId, string qry)
                => $"uom_ac_{ownerId:N}_{qry.ToMD5()}";

            public static string UomAllNormalizedNames(Guid ownerId)
                => $"uom_all_normalized_names_{ownerId:N}";

            public static string NomenclatureAutocomplete(Guid ownerId)
                => $"nom_ac_{ownerId:N}";

            public static string NomenclatureAutocompleteSearchInAlts(Guid ownerId, Guid clientId)
                => $"nom_ac_alt_{ownerId:N}_{clientId:N}";
            
            public static string NomenclatureCategoryCreateOrUpdate(Guid ownerId, string qry)
                => $"nc_cou_{ownerId:N}_{qry.ToMD5()}";
        }

        public static class Roles
        {
            public static string Admin = "Admin";
            public static string CompanyOwner = "CompanyOwner";
        }
    }
}

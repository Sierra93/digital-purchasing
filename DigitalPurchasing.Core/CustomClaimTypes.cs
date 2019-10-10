namespace DigitalPurchasing.Core
{
    public static class CustomClaimTypes
    {
        public static string CompanyId = "Custom:CompanyId";
        public static string CompanyName = "Custom:CompanyName";
        
        public static class SupplierOffers
        {
            public static string Delete = "Custom:SupplierOffers:Delete";
        }

        public static class User
        {
            public static string TimeZoneId = "Custom:User:TimeZone";
        }
    }
}

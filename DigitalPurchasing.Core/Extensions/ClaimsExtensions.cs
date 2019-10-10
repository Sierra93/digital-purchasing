using System;
using System.Security.Claims;

namespace DigitalPurchasing.Core.Extensions
{
    public static class ClaimsExtensions
    {
        public static Guid Id(this ClaimsPrincipal principal)
            => principal.HasClaim(q => q.Type == ClaimTypes.NameIdentifier)
                ? Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier).Value)
                : Guid.Empty;

        public static Guid CompanyId(this ClaimsPrincipal principal)
            => principal.HasClaim(q => q.Type == CustomClaimTypes.CompanyId)
                ? Guid.Parse(principal.FindFirst(CustomClaimTypes.CompanyId).Value)
                : Guid.Empty;

        public static bool CanDeleteSupplierOffers(this ClaimsPrincipal principal)
            => principal.HasClaim(q => q.Type == CustomClaimTypes.SupplierOffers.Delete);

        public static TimeZoneInfo GetUserTimeZoneInfo(this ClaimsPrincipal principal)
        {
            var tzId = principal.FindFirst(CustomClaimTypes.User.TimeZoneId).Value;
            return CustomTimeZoneConverter.GetTimeZoneInfo(tzId);
        }

        public static DateTime ToLocalTime(this ClaimsPrincipal principal, DateTime utcTime)
        {
            var tzi = GetUserTimeZoneInfo(principal);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
        }

        public static DateTime ToUtcTime(this ClaimsPrincipal principal, DateTime localTime)
        {
            var tzi = GetUserTimeZoneInfo(principal);
            return TimeZoneInfo.ConvertTimeToUtc(localTime, tzi);
        }
    }
}

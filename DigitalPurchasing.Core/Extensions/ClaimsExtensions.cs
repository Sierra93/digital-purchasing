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
    }
}

using System;
using System.Security.Claims;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DigitalPurchasing.Web.Core
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        public Tenant Get()
        {
            if (_httpContextAccessor == null)
            {
                return new Tenant { CompanyId = Guid.Empty, UserId = Guid.Empty };
            }

            if (_httpContextAccessor.HttpContext == null)
            {
                return new Tenant { CompanyId = Guid.Empty, UserId = Guid.Empty };
            }

            var claimUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var claimCompanyId = _httpContextAccessor.HttpContext.User.FindFirst(CustomClaimTypes.CompanyId);

            return new Tenant
            {
                CompanyId = claimCompanyId != null ? Guid.Parse(claimCompanyId.Value) : Guid.Empty,
                UserId = claimUserId != null ? Guid.Parse(claimUserId.Value) : Guid.Empty
            };
        }
    }
}

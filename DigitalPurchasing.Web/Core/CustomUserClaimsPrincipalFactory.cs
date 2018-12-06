using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Core.Interfaces;
using DigitalPurchasing.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DigitalPurchasing.Web.Core
{
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, Role>
    {
        private readonly ICompanyService _companyService;

        public CustomUserClaimsPrincipalFactory(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            ICompanyService companyService)
            : base(userManager, roleManager, optionsAccessor) => _companyService = companyService;

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim(CustomClaimTypes.CompanyId, user.CompanyId.ToString("N")));
            identity.AddClaim(new Claim(CustomClaimTypes.CompanyName, _companyService.GetByUser(user.Id).Name ?? string.Empty));
            return identity;
        }
    }
}

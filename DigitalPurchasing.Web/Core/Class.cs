using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DigitalPurchasing.Core;
using DigitalPurchasing.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DigitalPurchasing.Web.Core
{
    
    public class VueTableRequest
    {
        [FromQuery(Name="sort")]
        public string Sort { get; set; }

        [FromQuery(Name="page")]
        public int Page { get; set; }

        [FromQuery(Name="per_page")]
        public int PerPage { get; set; }

        public VueTableRequest NextPageRequest()
        {
            var next = this;
            next.Page += 1;
            return next;
        }

        public VueTableRequest PrevPageRequest()
        {
            var prev = this;
            prev.Page -= 1;
            if (prev.Page <= 0) prev.Page = 1;
            return prev;
        }

        public string SortField
        {
            get
            {
                if (string.IsNullOrEmpty(Sort)) return null;
                return Sort.Split('|')[0];
            }
        }

        public bool SortAsc
        {
            get
            {
                if (string.IsNullOrEmpty(Sort)) return true;
                return Sort.Split('|')[1] == "asc";
            }
        }
    }

    public class VueTableResponse<TData> where TData: class
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }

        [JsonProperty("current_page")]
        public int CurrentPage { get; set; }

        [JsonProperty("last_page")]
        public int LastPage { get; set; }

        [JsonProperty("next_page_url")]
        public string NextPageUrl { get; set; }

        [JsonProperty("prev_page_url")]
        public string PrevPageUrl { get; set; }

        [JsonProperty("from")]
        public int From { get; set; }

        [JsonProperty("to")]
        public int To { get; set; }

        [JsonProperty("data")]
        public List<TData> Data { get; set; }

        public VueTableResponse(List<TData> data, VueTableRequest request, int total, string nextPageUrl, string prevPageUrl)
        {
            Data = data;
            CurrentPage = request.Page;
            PerPage = request.PerPage;
            Total = total;

            var lastPage = (total + request.PerPage - 1) / request.PerPage;
            var from = ( request.Page - 1 ) * request.PerPage + 1;
            var to = from + request.PerPage;
            if (to > total)
            {
                to = total;
            }

            LastPage = lastPage;
            From = from;
            To = to;

            if (request.Page > 1)
            {
                PrevPageUrl = prevPageUrl;
            }

            if (request.Page < lastPage)
            {
                NextPageUrl = nextPageUrl;
            }
        }
    }

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

    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, Role>
    {
        public CustomUserClaimsPrincipalFactory(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor) {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim(CustomClaimTypes.CompanyId, user.CompanyId.ToString("N")));
            return identity;
        }
    }
}

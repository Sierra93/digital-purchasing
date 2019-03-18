using DigitalPurchasing.Core;
using Hangfire.Dashboard;

namespace DigitalPurchasing.Web.Core
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var isAdmin = httpContext.User.IsInRole(Consts.Roles.Admin);
            return isAdmin;
        }
    }
}

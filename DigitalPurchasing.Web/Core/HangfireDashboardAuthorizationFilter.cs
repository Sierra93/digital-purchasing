using Hangfire.Dashboard;

namespace DigitalPurchasing.Web.Core
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return true;
            //return httpContext.User.IsAdmin();
        }
    }
}

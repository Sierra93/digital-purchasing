using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging;

namespace DigitalPurchasing.Web.Core
{
    public class HangfireSentryAttribute : JobFilterAttribute, IElectStateFilter
    {
        public void OnStateElection(ElectStateContext context)
        {
            if (context.CandidateState is FailedState failedState)
            {
                using (var loggerFactory = new LoggerFactory().AddSentry(o => o.InitializeSdk = false))
                {
                    var logger = loggerFactory.CreateLogger("Hangfire");
                    logger.LogError(failedState.Exception, $"Job '{context.BackgroundJob.Id}' has been failed due to an exception");
                }
            }
        }
    }
}

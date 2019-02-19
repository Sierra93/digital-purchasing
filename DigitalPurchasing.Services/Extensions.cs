using Mandrill;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalPurchasing.Services
{
    public static class Extensions
    {
        public static void AddMandrill(this IServiceCollection services)
        {
            var api = new MandrillApi("qbm_XHdv5CerLGRZJpYWfQ");
            services.AddSingleton(api.Messages);
        }
    }
}

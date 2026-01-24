using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces;

namespace Services.Shared.DI
{
    internal static class SharedServiceRegistration
    {
        public static void RegisterSharedServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IHttpClient<>, typeof(ApiHttpClient<>));
           
        }
    }
}

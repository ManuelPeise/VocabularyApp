using Logic.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Services.Shared.Interfaces;
using Shared.Interfaces;

namespace Services.Shared.DI
{
    public static class SharedServiceRegistration
    {
        public static void RegisterSharedServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IHttpClient<>), typeof(ApiHttpClient<>));
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ISecureStorageHandler, SecureStorageHandler>();
        }
    }
}

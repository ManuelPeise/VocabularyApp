using Microsoft.Extensions.DependencyInjection;
using Services.Shared.Interfaces;
using Services.Shared.UnitOfWorks;
using Services.Shared.UserServices;

namespace Services.Shared.DI
{
    public static class SharedServiceRegistration
    {
        public static void RegisterSharedServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IHttpClient<>), typeof(ApiHttpClient<>));
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ISecureStorageHandler, SecureStorageHandler>();
            services.AddScoped<IUserAdministrationService, UserAdministrationService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddSingleton<ILocalizationResourceManager, LocalizationResourceManager>();
        }
    }
}

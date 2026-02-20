using Logic.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Administration.DI
{
    public static class AdministrationServiceRegistration
    {
        public static void RegisterAdministrationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserAdministrationService, UserAdministrationService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
        }
    }
}

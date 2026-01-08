using Logic.Administration.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Administration.DI
{
    public static class AdministrationServiceRegistration
    {
        public static void RegisterAdministrationServices(this IServiceCollection services)
        {
            services.AddScoped<Interfaces.IUserAuthentication, UserAuthentication>();
            services.AddScoped<IUserAdministration, UserAdministration>();
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
        }
    }
}

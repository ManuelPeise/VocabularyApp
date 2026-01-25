using Logic.Shared.Interfaces;
using Logic.Shared.UnitsOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Shared.DI
{
    public static class LogicSharedServiceRegistration
    {

        public static void RegisterLogicSharedServices(this IServiceCollection services)
        {
            services.AddScoped<IAdministrationUnitOfWork, AdministrationUnitOfWork>();
        }
    }
}

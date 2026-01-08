using Core.App.ViewModels;
using Core.App.Views.Private.User;
using Core.App.Views.Public;
using Data.Accessor.DI;
using Logic.Administration.DI;
using Logic.Shared.DI;

namespace Core.App.Bundles
{
    internal static class ServiceRegistration
    {
        internal static void RegisterAppServices(this MauiAppBuilder builder)
        {
            DataAccessorServiceRegistration.RegisterDataAccessorServices(builder.Services);
            LogicSharedServiceRegistration.RegisterSharedServices(builder.Services);
            AdministrationServiceRegistration.RegisterAdministrationServices(builder.Services);
        }

        internal static void RegisterViewModels(this IServiceCollection services)
        {
            services.AddTransient<StartPageViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<HomePageViewModel>();
        }

        internal static void RegisterViews(this IServiceCollection services)
        {
            services.AddTransient<StartPage>();
            services.AddTransient<LoginPage>();
            services.AddTransient<RegisterPage>();
            services.AddTransient<HomePage>();
        }
    }
}

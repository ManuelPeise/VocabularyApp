using Core.App.ViewModels;
using Core.App.Views.Private.User;
using Core.App.Views.Private.User.Profile;
using Core.App.Views.Public;
using Data.Accessor.DI;
using Logic.Administration.DI;
using Logic.Shared.DI;
using Services.Shared.DI;

namespace Core.App.Bundles
{
    internal static class ServiceRegistration
    {
        internal static void RegisterAppServices(this MauiAppBuilder builder)
        {
            builder.Services.RegisterDataAccessorServices();
            builder.Services.RegisterLogicSharedServices();
            builder.Services.RegisterSharedServices();
            builder.Services.RegisterAdministrationServices();

        }

        internal static void RegisterViewModels(this IServiceCollection services)
        {
            services.AddTransient<PrivateViewModelBase>();
            services.AddTransient<StartPageViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<HomePageViewModel>();
            services.AddTransient<UserProfilePageViewModel>();
            services.AddTransient<ChangePasswordViewModel>();
        }

        internal static void RegisterViews(this IServiceCollection services)
        {
            services.AddTransient<App>();
            services.AddTransient<StartPage>();
            services.AddTransient<LoginPage>();
            services.AddTransient<RegisterPage>();
            services.AddTransient<HomePage>();
            services.AddTransient<UserProfilePage>();
            services.AddTransient<ChangePasswordPopup>();
        }
    }
}

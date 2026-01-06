using Data.Accessor.DI;
using Logic.Shared.DI;

namespace Core.App.Bundles
{
    internal static class ServiceRegistration
    {
        internal static void RegisterAppServices(this MauiAppBuilder builder)
        {
            DataAccessorServiceRegistration.RegisterDataAccessorServices(builder.Services);
            LogicSharedServiceRegistration.RegisterSharedServices(builder.Services);
        }
    }
}

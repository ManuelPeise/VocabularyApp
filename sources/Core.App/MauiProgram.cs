using CommunityToolkit.Maui;
using Core.App.Bundles;
using Logic.Administration.DI;
using Logic.Shared.DI;
using Microsoft.Extensions.Logging;

namespace Core.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fa-solid-900.ttf", "FASolid");
                });

            Database.RegisterDatabaseServices(builder);
           
            builder.RegisterAppServices();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            Database.Migrate(app);

            return app;
        }
    }
}

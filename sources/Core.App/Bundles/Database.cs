using Data.Database;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Core.App.Bundles
{
    internal static class Database
    {
        internal static void RegisterDatabaseServices(MauiAppBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                string dbPath;

#if ANDROID
                dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    "VocabularyAppDb.db");
#elif WINDOWS
                dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "VocabularyAppDb.db");
#else
                dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "VocabularyAppDb.db");
#endif

                // Sicherstellen, dass das Verzeichnis existiert
                var dbDirectory = Path.GetDirectoryName(dbPath);
                
                if (!Directory.Exists(dbDirectory))
                {
                    Directory.CreateDirectory(dbDirectory!);
                }

                opt.UseSqlite($"Data Source={dbPath}");
            });
        }

        internal static void Migrate(MauiApp app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (db.Database.GetPendingMigrations().Any())
                {
                    db.Database.Migrate();
                }
            }
        }
    }
}

using Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Core.App.Bundles
{
    internal static class Database
    {
        internal static void RegisterDatabaseServices(MauiAppBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "VocabularyAppDb.db");
                
                opt.UseSqlite($"Data Source={dbPath}");
            });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data.Database
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            SQLitePCL.Batteries.Init();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var dbPath = args.Length > 0
           ? args[0]
           : Path.Combine(AppContext.BaseDirectory, "VocabularyAppDb.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

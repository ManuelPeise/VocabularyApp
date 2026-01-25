using Data.Database.Entities;
using Data.Database.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Data.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

        public DbSet<UserEntity> UserTable { get; set; }
        public DbSet<UserCredentialsEntity> UserCredentialsTable { get; set; }
        public DbSet<LogMessageEntity> LogMessageTable { get; set; }
        public DbSet<UserSettingsEntity> UserSettingsTable { get; set; }
    }
}

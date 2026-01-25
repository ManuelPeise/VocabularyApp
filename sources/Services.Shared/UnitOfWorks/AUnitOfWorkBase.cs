using Data.Accessor;
using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Services.Shared.UnitOfWorks
{
    public abstract class AUnitOfWorkBase
    {
        protected readonly AppDbContext _appDbContext;

        protected AUnitOfWorkBase(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        protected IRepositoryBase<T> CreateRepository<T>() where T : AEntityBase
        {
            if (_appDbContext == null) throw new ObjectDisposedException(nameof(AUnitOfWorkBase));
            return new RepositoryBase<T>(_appDbContext);
        }
        protected async Task<int> SaveChangesAsync(string userName)
        {
            if (_appDbContext == null) throw new ObjectDisposedException(nameof(AUnitOfWorkBase));

            var now = DateTime.UtcNow;

            var entries = _appDbContext.ChangeTracker.Entries<AEntityBase>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.CreatedBy = userName ?? string.Empty;
                    entry.Entity.UpdatedBy = userName ?? string.Empty;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(AEntityBase.CreatedAt)).IsModified = false;
                    entry.Property(nameof(AEntityBase.CreatedBy)).IsModified = false;

                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userName ?? "System";
                }
            }

            return await _appDbContext.SaveChangesAsync();
        }

        protected async Task LogMessage(LogMessageEntity logMessageEntity, bool save = false, string? userName = null)
        {
            _appDbContext.LogMessageTable.Add(logMessageEntity);

            if (save)
            {
                await SaveChangesAsync(userName ?? "System");
            }
        }
    }
}

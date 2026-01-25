using Data.Accessor.Interfaces;
using Data.Database.Entities;
using Data.Database.Entities.User;
using System.Linq.Expressions;

namespace Logic.Shared.Interfaces
{
    public interface IAdministrationUnitOfWork
    {
        IRepositoryBase<LogMessageEntity> LogRepository { get; }
        IRepositoryBase<UserEntity> UserRepository { get; }
        IRepositoryBase<UserCredentialsEntity> UserCredentialsRepository { get; }
        IRepositoryBase<UserSettingsEntity> UserSettingsRepository { get; }
        Task CommittChanges(string userName);
        Task<IEnumerable<LogMessageEntity>> GetAllLogMessagesAsync(Expression<Func<LogMessageEntity, bool>>? expression = null);
    }
}

using Data.Accessor.Interfaces;
using Data.Database.Entities;
using System.Linq.Expressions;

namespace Logic.Shared.Interfaces
{
    public interface IAdministrationUnitOfWork
    {
        IRepositoryBase<LogMessageEntity> LogRepository { get; }
        IRepositoryBase<UserEntity> UserRepository { get; }
        IRepositoryBase<UserCredentialsEntity> UserCredentialsRepository { get; }
        Task<IEnumerable<LogMessageEntity>> GetAllLogMessagesAsync(Expression<Func<LogMessageEntity, bool>>? expression = null);
    }
}

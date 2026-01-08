using Data.Accessor.Interfaces;
using Data.Database;
using Data.Database.Entities;
using Logic.Shared.Interfaces;
using System.Linq.Expressions;

namespace Logic.Shared.UnitsOfWork
{
    public class AdministrationUnitOfWork : AUnitOfWorkBase, IAdministrationUnitOfWork
    {
        private readonly IRepositoryBase<LogMessageEntity> _logRepository;
        public IRepositoryBase<LogMessageEntity> LogRepository => _logRepository;

        private readonly IRepositoryBase<UserEntity> _userRepository;
        public IRepositoryBase<UserEntity> UserRepository => _userRepository;

        private readonly IRepositoryBase<UserCredentialsEntity> _userCredentialsRepository;
        public IRepositoryBase<UserCredentialsEntity> UserCredentialsRepository => _userCredentialsRepository;

        public AdministrationUnitOfWork(AppDbContext appDbContext) : base(appDbContext)
        {
            _logRepository = CreateRepository<LogMessageEntity>();
            _userRepository = CreateRepository<UserEntity>();
            _userCredentialsRepository = CreateRepository<UserCredentialsEntity>();
        }

        public async Task<IEnumerable<LogMessageEntity>> GetAllLogMessagesAsync(Expression<Func<LogMessageEntity, bool>>? expression = null)
        {
            if (expression != null)
            {
                return await _logRepository.GetAllByAsync(expression);
            }

            return await _logRepository.GetAllAsync();
        }

        public async Task CommittChanges(string userName) => await SaveChangesAsync(userName);
        
    }
}

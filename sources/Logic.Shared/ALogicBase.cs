using Data.Accessor.Interfaces;
using Data.Database.Entities;
using System.Text;

namespace Logic.Shared
{
    public abstract class ALogicBase
    {
        private readonly IRepositoryBase<LogMessageEntity> _logRepository;
        private Func<string, Task> _committChanges;

        protected ALogicBase(IRepositoryBase<LogMessageEntity> logRepository, Func<string, Task> committChanges)
        {
            _logRepository = logRepository;
            _committChanges = committChanges;
        }

        protected async Task LogMessageAsync(LogMessageEntity logEntity)
        {
           
            await _logRepository.AddAsync(logEntity);

            await _committChanges("System");
        }
    }
}

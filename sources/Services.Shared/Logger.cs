using Data.Database;
using Data.Database.Entities;
using Services.Shared.Interfaces;
using Shared.Enums;

namespace Services.Shared
{
    public class Logger<T> : ILogger<T> where T : class
    {
        private readonly AppDbContext _context;

        public Logger(AppDbContext context)
        {
            _context = context;

        }

        public async Task LogMessageAsync(string message, LogMessageTypeEnum type, Exception? exception = null)
        {
            var logEntry = new LogMessageEntity
            {
                Message = message,
                ExeptionMessage = exception?.Message,
                Stacktrace = exception?.StackTrace,
                LogLevel = type,
                Module = typeof(T).Name,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow
            };

            _context.LogMessageTable.Add(logEntry);

            await _context.SaveChangesAsync();
        }
    }
}

using Shared.Enums;

namespace Services.Shared.Interfaces
{
    public interface ILogger<T> where T : class
    {
        Task LogMessageAsync(string message, LogMessageTypeEnum type, Exception? exception = null);
    }
}

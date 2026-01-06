using Shared.Enums;

namespace Data.Database.Entities
{
    public class LogMessageEntity: AEntityBase
    {
        public string Message { get; set; } = string.Empty;
        public string? ExeptionMessage { get; set; }
        public string? Stacktrace { get; set; }
        public string Module { get; set; } = string.Empty;
        public LogLevelEnum LogLevel { get; set; }
    }
}

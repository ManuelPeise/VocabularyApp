using System.Text.Json.Serialization;

namespace Services.Shared.Models.User
{
    public class UserSettings
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("useLocalDataStore")]
        public bool UseLocalDataStore { get; set; }
        [JsonPropertyName("isAutoDataSyncEnabled")]
        public bool IsAutoDataSyncEnabled { get; set; }
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = string.Empty;
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updatedBy")]
        public string UpdatedBy { get; set; } = string.Empty;
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}

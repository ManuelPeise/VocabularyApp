using System.Text.Json.Serialization;

namespace Data.Database.Entities.User
{
    public class UserSettingsEntity:AEntityBase
    {
        [JsonPropertyName("useLocalDataStore")]
        public bool UseLocalDataStore { get; set; }
        [JsonPropertyName("isAutoDataSyncEnabled")]
        public bool IsAutoDataSyncEnabled { get; set; }
    }
}

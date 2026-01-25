using System.Text.Json.Serialization;

namespace Data.Database.Entities.User
{
    public class UserCredentialsEntity: AEntityBase
    {
        [JsonPropertyName("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;
        [JsonPropertyName("refreshToken")]
        public string? RefreshToken { get; set; }
        [JsonPropertyName("expireDate")]
        public DateTime ExpireDate { get; set; }
    }
}

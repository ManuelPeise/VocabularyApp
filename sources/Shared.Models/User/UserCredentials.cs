using System.Text.Json.Serialization;

namespace Shared.Models.User
{
    public class UserCredentials
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;
        [JsonPropertyName("refreshToken")]
        public string? RefreshToken { get; set; }
        [JsonPropertyName("expireDate")]
        public DateTime ExpireDate { get; set; }
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

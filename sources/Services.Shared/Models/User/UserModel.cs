using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Services.Shared.Models.User
{
    public class UserModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("userIdExternal")]
        public string UserIdExternal { get; set; } = string.Empty;
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; } = string.Empty;
        [JsonPropertyName("profileImage")]
        public byte[] ProfileImage { get; set; } = Array.Empty<byte>();
        [JsonPropertyName("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }
        [JsonPropertyName("userRole")]
        public UserRoleEnum UserRole { get; set; }
        [JsonPropertyName("userCredentialsId")]
        public int UserCredentialsId { get; set; }
        [JsonPropertyName("userCredentials")]
        public UserCredentials? UserCredentials { get; set; }
        [JsonPropertyName("userSettingsId")]
        public int UserSettingsId { get; set; }
        [JsonPropertyName("userSettings")]
        public UserSettings? UserSettings { get; set; }
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

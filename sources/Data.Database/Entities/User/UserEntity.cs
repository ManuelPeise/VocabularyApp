using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Data.Database.Entities.User
{
    public class UserEntity : AEntityBase
    {
        [JsonPropertyName("userIdExternal")]
        public string UserIdExternal { get; set; } = string.Empty;
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
        [JsonPropertyName("userName")]
        public string UserName => EmailAddress;
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
        [ForeignKey(nameof(UserCredentialsId))]
        [JsonPropertyName("userCredentials")]
        public UserCredentialsEntity? UserCredentials { get; set; }
        [JsonPropertyName("userSettingsId")]
        public int UserSettingsId { get; set; }
        [ForeignKey(nameof(UserSettingsId))]
        [JsonPropertyName("userSettings")]
        public UserSettingsEntity? UserSettings { get; set; }
    }
}

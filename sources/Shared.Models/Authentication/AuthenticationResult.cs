using System.Text.Json.Serialization;

namespace Shared.Models.Authentication
{
    public class AuthenticationResult
    {
        /// <summary>
        /// Indicates whether authentication was successful.
        /// </summary>
        [JsonPropertyName("result")]
        public bool Result { get; set; }

        /// <summary>
        /// JWT access token for authenticated API requests.
        /// </summary>
        [JsonPropertyName("accessToken")]
        public string? AccessToken { get; set; }

        /// <summary>
        /// Refresh token for obtaining new access tokens.
        /// Note: API has typo "refeshToken" instead of "refreshToken".
        /// </summary>
        [JsonPropertyName("refeshToken")]
        public string? RefreshToken { get; set; }
    }
}

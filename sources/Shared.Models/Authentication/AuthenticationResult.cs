using System.Text.Json.Serialization;

namespace Shared.Models.Authentication
{
    public class AuthenticationResult
    {
        public bool Result { get; set; }

        public string? AccessToken { get; set; }

        public string? RefeshToken { get; set; }
    }
}

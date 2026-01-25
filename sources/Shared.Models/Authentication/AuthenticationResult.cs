namespace Shared.Models.Authentication
{
    public class AuthenticationResult
    {
        public bool Result { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}

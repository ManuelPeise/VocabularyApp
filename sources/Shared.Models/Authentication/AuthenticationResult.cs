using Shared.Enums;

namespace Shared.Models.Authentication
{
    public class AuthenticationResult
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public UserRoleEnum UserRole { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}

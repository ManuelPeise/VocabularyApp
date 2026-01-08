using Shared.Enums;
using Shared.Models.Authentication;

namespace Logic.Administration.Interfaces
{
    public interface ICurrentUserService
    {
        public int UserId { get; }
        public string UserName { get; }
        public UserRoleEnum? UserRole { get; }
        public bool IsAuthenticated { get; }
        public void SetCurrentUser(AuthenticationResult authenticationResult);
    }
}

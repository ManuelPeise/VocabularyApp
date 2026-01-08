using Logic.Administration.Interfaces;
using Shared.Enums;
using Shared.Models.Authentication;

namespace Logic.Administration
{
    public class CurrentUserService : ICurrentUserService
    {
        private AuthenticationResult _currentUser;

        public CurrentUserService()
        {
            _currentUser = new AuthenticationResult
            {
                IsAuthenticated = false
            };
        }

        public int UserId => _currentUser.UserId == 0 ? -1 : _currentUser.UserId;
        public string UserName => _currentUser.UserName ?? string.Empty;
        public UserRoleEnum? UserRole => _currentUser.UserRole;
        public bool IsAuthenticated => _currentUser.IsAuthenticated;

        public void SetCurrentUser(AuthenticationResult authenticationResult)
        {
            _currentUser = authenticationResult;
        }
    }
}

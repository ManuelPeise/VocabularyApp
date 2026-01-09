using Shared.Enums;
using Shared.Models.Authentication;

namespace Logic.Administration.Interfaces
{
    public interface ICurrentUserService
    {
        public AuthenticationResult AuthenticationResult { get; }
        public void SetCurrentUser(AuthenticationResult authenticationResult);
    }
}

using Shared.Models.Authentication;
using Shared.Models.User;

namespace Logic.Shared.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> AuthenticateUser(AuthenticationRequestModel authRequest, CurrentUser? currentUser);
        Task SignOutAsync(CurrentUser? userData);
        Task<CurrentUser?> GetCurrentUser();
    }
}

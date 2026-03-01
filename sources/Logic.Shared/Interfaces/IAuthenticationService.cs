using Shared.Models.Authentication;
using Shared.Models.User;

namespace Logic.Shared.Interfaces
{
    public interface IAuthenticationService
    {
        Task<CurrentUser?> AuthenticateUser(AuthenticationRequestModel authRequest);
        Task SignOutAsync(CurrentUser? userData);
        Task<CurrentUser?> GetCurrentUser();
    }
}

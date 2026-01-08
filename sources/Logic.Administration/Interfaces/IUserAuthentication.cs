using Shared.Models.Authentication;

namespace Logic.Administration.Interfaces
{
    public interface IUserAuthentication
    {
        Task<AuthenticationResult> AuthenticateUser(AuthenticationRequestModel authenticationRequestModel);
    }
}

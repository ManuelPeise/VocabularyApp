using Services.Shared.UiModels;
using Shared.Models.Authentication;

namespace Services.Shared.Interfaces
{
    public interface ICurrentUserService
    {
        public UserData UserData { get; set; }
        Task<bool> AuthenticateUser(AuthenticationRequestModel authData);
        Task SignOutAsync();
    }
}

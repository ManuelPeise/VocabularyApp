using Shared.Models.Authentication;
using Shared.Models.User;

namespace Logic.Shared.Interfaces
{
    public interface ICurrentUserService
    {
        public CurrentUser UserData { get; set; }
        Task<bool> AuthenticateUser(AuthenticationRequestModel authData);
        Task SignOutAsync();
        Task<UserSettingsModel> GetCurrentUserSettings(int userId);
        Task UpdateUserSettings(UserSettingsModel model);

    }
}

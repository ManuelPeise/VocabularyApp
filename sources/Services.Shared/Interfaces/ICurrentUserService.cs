using Services.Shared.UiModels;

namespace Services.Shared.Interfaces
{
    public interface ICurrentUserService
    {
        public UserData UserData { get; set; }
        Task<bool> AuthenticateUser(AuthenticationRequestModel authData);
        Task SignOutAsync();
        Task<UserSettingsModel> GetCurrentUserSettings(int userId);
        Task UpdateUserSettings(UserSettingsModel model);
    }
}

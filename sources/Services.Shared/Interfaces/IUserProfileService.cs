using Services.Shared.UiModels;

namespace Services.Shared.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileModel?> GetProfile(int userId);

        Task<UserProfileModel?> UpdateProfile(UserProfileModel profile);
    }
}

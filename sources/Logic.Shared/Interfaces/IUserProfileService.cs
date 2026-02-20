using Shared.Models.Authentication;
using Shared.Models.User;

namespace Logic.Shared.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileModel?> GetProfile(int userId);
        Task<UserProfileModel?> UpdateProfile(UserProfileModel profile);
        Task<ChangePasswordResult> ChangePassword(ChangePasswordModel changePasswordModel);
    }
}

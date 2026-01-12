using Shared.Models.UserAdministration;

namespace Logic.Administration.Interfaces
{
    public interface IUserAdministration
    {
        Task<UserRegistrationResult> CreateUserProfile(UserRegistrationRequestModel registrationRequestModel);
        Task<UserProfile?> LoadUserProfileAsync(int userId);
        Task<UserProfile?> UpdateUserProfile(UserProfile profile);
        Task<ChangePasswordResult> ChangePassword(ChangePasswordModel model);
    }
}

using Shared.Models.UserAdministration;

namespace Logic.Administration.Interfaces
{
    public interface IUserAdministration
    {
        Task<UserRegistrationResult> CreateUserProfile(UserRegistrationRequestModel registrationRequestModel);
    }
}

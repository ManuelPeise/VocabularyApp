using Shared.Models.Authentication;

namespace Logic.Shared.Interfaces
{
    public interface IUserAdministrationService
    {
        Task<UserRegistrationResult> CreateUserProfile(UserRegistrationRequestModel registrationRequestModel);
    }
}

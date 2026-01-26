using Services.Shared.Models;
using Services.Shared.UiModels;

namespace Services.Shared.Interfaces
{
    public interface IUserAdministrationService
    {
        Task<UserRegistrationResult> CreateUserProfile(UserRegistrationRequestModel registrationRequestModel);
    }
}

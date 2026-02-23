using Data.Accessor.Interfaces;
using Data.Database.Entities.User;
using Logic.Shared;
using Logic.Shared.Interfaces;
using Shared.Enums;
using Shared.Models.Authentication;

namespace Logic.Administration
{
    public class UserAdministrationService : IUserAdministrationService
    {
        private readonly ILogger<UserAdministrationService> _logger;
        private readonly IUnitOfWork _administrationUnitOfWork;
        private readonly IHttpClient _httpClient;
        public UserAdministrationService(ILogger<UserAdministrationService> logger, IUnitOfWork administrationUnitOfWork, IHttpClient httpClient)
        {
            _administrationUnitOfWork = administrationUnitOfWork;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<UserRegistrationResult> CreateUserProfile(UserRegistrationRequestModel registrationRequestModel)
        {
            try
            {
                var isValidModel = await IsValidRegistrationRequestModel(registrationRequestModel);

                if (!isValidModel)
                {
                    return new UserRegistrationResult { Result = false };
                }

                if (!await _httpClient.IsApiAvailableAsync)
                {
                    return new UserRegistrationResult { Result = false, Message = "Api is not reachable." };
                }

                var response = await _httpClient.SendPostRequest("useradministration/registeruser", null, registrationRequestModel);

                response.EnsureSuccessStatusCode();

                return new UserRegistrationResult { Result = true };
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync("Create user profile failed.", LogMessageTypeEnum.Error, exception);

                return new UserRegistrationResult { Result = false };
            }
        }

        private async Task<bool> IsValidRegistrationRequestModel(UserRegistrationRequestModel registrationRequestModel)
        {
            var existingUser = await _administrationUnitOfWork.UserRepository.FirstOrDefaultAsync(x => x.EmailAddress == registrationRequestModel.EmailAddress);

            if (existingUser != null)
            {
                return false;
            }

            return string.IsNullOrEmpty(registrationRequestModel.FirstName) ||
                   string.IsNullOrEmpty(registrationRequestModel.LastName) ||
                   string.IsNullOrEmpty(registrationRequestModel.EmailAddress) ||
                   string.IsNullOrEmpty(registrationRequestModel.Password) &&
                   registrationRequestModel.Password.Length > 6 ? false : true;
        }
    }
}

using Data.Database;
using Data.Database.Entities.User;
using Services.Shared.Interfaces;
using Services.Shared.Models;
using Services.Shared.UiModels;
using Shared.Enums;

namespace Services.Shared
{
    public class UserAdministrationService : IUserAdministrationService
    {
        private readonly Logger<UserAdministrationService> _logger;
        private readonly IUnitOfWork _administrationUnitOfWork;

        public UserAdministrationService(AppDbContext dbContext, IUnitOfWork administrationUnitOfWork)
        {
            _administrationUnitOfWork = administrationUnitOfWork;
            _logger = new Logger<UserAdministrationService>(dbContext);
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

                var userIdExternal = Guid.NewGuid().ToString();

                var entity = new UserEntity
                {
                    UserIdExternal = userIdExternal,
                    FirstName = registrationRequestModel.FirstName,
                    LastName = registrationRequestModel.LastName,
                    EmailAddress = registrationRequestModel.EmailAddress,
                    ProfileImage = [],
                    DateOfBirth = registrationRequestModel.DateOfBirth,
                    UserRole = UserRoleEnum.User,
                    UserCredentials = new UserCredentialsEntity
                    {
                        PasswordHash = PasswordHasher.HashPassword(registrationRequestModel.Password),
                    },
                    UserSettings = new UserSettingsEntity
                    {
                        IsAutoDataSyncEnabled = false,
                        UseLocalDataStore = false,
                    },
                };

                await _administrationUnitOfWork.UserRepository.AddAsync(entity, x => x.UserName == entity.UserName && x.UserIdExternal == userIdExternal);

                await _administrationUnitOfWork.CommittChanges("System");


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

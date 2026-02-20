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

        public UserAdministrationService(ILogger<UserAdministrationService> logger, IUnitOfWork administrationUnitOfWork)
        {
            _administrationUnitOfWork = administrationUnitOfWork;
            _logger = logger;
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

                var idExternal = Guid.NewGuid();

                var entity = new UserEntity
                {
                    IdExternal = idExternal,
                    FirstName = registrationRequestModel.FirstName,
                    LastName = registrationRequestModel.LastName,
                    EmailAddress = registrationRequestModel.EmailAddress,
                    ProfileImage = [],
                    DateOfBirth = registrationRequestModel.DateOfBirth,
                    UserRole = UserRoleEnum.User,
                    UserCredentials = new UserCredentialsEntity
                    {
                        IdExternal = Guid.NewGuid(),
                        PasswordHash = PasswordHasher.HashPassword(registrationRequestModel.Password),
                        ExpireDate = DateTime.UtcNow.AddDays(30),
                        RefreshToken = null,
                        IsDirty = true,
                    },
                    UserSettings = new UserSettingsEntity
                    {
                        IdExternal = Guid.NewGuid(),
                        IsAutoDataSyncEnabled = false,
                        UseLocalDataStore = false,
                        Culture = CultureEnum.English,
                        IsDirty = true,
                    },
                };

                await _administrationUnitOfWork.UserRepository.AddAsync(entity, x => x.UserName == entity.UserName && x.IdExternal == idExternal);

                await _administrationUnitOfWork.SaveChangesAsync("System");


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

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

        public async Task<UserProfileModel?> LoadUserProfileAsync(int userId)
        {
            try
            {
                var userEntity = await _administrationUnitOfWork.UserRepository.FirstOrDefaultByIdAsync(userId, false);

                if (userEntity == null)
                {
                    throw new Exception($"Could not load user profile [{userId}]");
                }

                return new UserProfileModel
                {
                    UserId = userEntity.Id,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    UserName = userEntity.UserName,
                    ProfileImage = userEntity.ProfileImage,
                    DateOfBirth = userEntity.DateOfBirth,
                    UserRole = userEntity.UserRole,
                    LastUpdateBy = string.IsNullOrEmpty(userEntity.UpdatedBy) ? userEntity.CreatedBy : userEntity.UpdatedBy,
                    LastUpdateAt = userEntity.UpdatedAt == DateTime.MinValue ? userEntity.CreatedAt.ToString("dd.MM.yyyy HH:mm:ss") : userEntity.UpdatedAt.ToString("dd.MM.yyyy HH:mm:ss"),
                };


            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync($"Could not load user profile [{userId}]", LogMessageTypeEnum.Error, exception);

                return null;
            }
        }

        public async Task<UserProfileModel?> UpdateUserProfile(UserProfileModel profile)
        {
            try
            {
                var userEntity = await _administrationUnitOfWork.UserRepository.FirstOrDefaultAsync(x => x.Id == profile.UserId, false);

                if (userEntity == null)
                {
                    throw new Exception($"Could not update user profile image [{profile.UserId}]");
                }

                userEntity.FirstName = profile.FirstName;
                userEntity.LastName = profile.LastName;
                userEntity.EmailAddress = profile.Email;
                userEntity.DateOfBirth = profile.DateOfBirth;
                userEntity.ProfileImage = profile.ProfileImage;
                userEntity.UpdatedAt = DateTime.UtcNow;
                userEntity.UpdatedBy = profile.UserName;

                await _administrationUnitOfWork.CommittChanges(userEntity.UserName);

                return new UserProfileModel
                {
                    UserId = userEntity.Id,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    UserName = userEntity.UserName,
                    ProfileImage = userEntity.ProfileImage,
                    DateOfBirth = userEntity.DateOfBirth,
                    UserRole = userEntity.UserRole,
                    LastUpdateBy = string.IsNullOrEmpty(userEntity.UpdatedBy) ? userEntity.CreatedBy : userEntity.UpdatedBy,
                    LastUpdateAt = userEntity.UpdatedAt == DateTime.MinValue ? userEntity.CreatedAt.ToString("dd.MM.yyyy HH:mm:ss") : userEntity.UpdatedAt.ToString("dd.MM.yyyy HH:mm:ss"),
                };
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync($"Could not update user profile image [{profile.UserId}]", LogMessageTypeEnum.Error, exception);

                return null;
            }
        }

        public async Task<ChangePasswordResult> ChangePassword(ChangePasswordModel model)
        {
            try
            {
                var userEntity = await _administrationUnitOfWork.UserRepository.FirstOrDefaultByIdAsync(model.UserId, false, x => x.UserCredentials);

                if (userEntity == null || userEntity.UserCredentials == null)
                {
                    throw new Exception($"Cannot find user entity to change password [{model.UserId}].");
                }

                if (userEntity.UserCredentials.PasswordHash != PasswordHasher.HashPassword(model.CurrentPassword))
                {
                    return new ChangePasswordResult
                    {
                        Success = false,
                        ErrorMessage = Resx.Core.LabelCurrentPasswordDoesNotMatch
                    };
                }

                if (PasswordHasher.HashPassword(model.NewPassword) != PasswordHasher.HashPassword(model.PasswordReplication))
                {
                    return new ChangePasswordResult
                    {
                        Success = false,
                        ErrorMessage = Resx.Core.LabelPasswordsDoesNotMatch
                    };
                }

                userEntity.UserCredentials.PasswordHash = PasswordHasher.HashPassword(model.NewPassword);

                await _administrationUnitOfWork.CommittChanges(userEntity.UserName);

                return new ChangePasswordResult
                {
                    Success = true,
                    ErrorMessage = string.Empty
                };
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync($"Cannot find user entity to change password [{model.UserId}].", LogMessageTypeEnum.Error, exception);

                return new ChangePasswordResult
                {
                    Success = false,
                    ErrorMessage = Resx.Core.LabelChangePasswordFaild
                };
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

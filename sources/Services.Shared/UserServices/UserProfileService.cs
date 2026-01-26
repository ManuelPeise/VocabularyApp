using Data.Database;
using Data.Database.Entities.User;
using Services.Shared.Interfaces;
using Services.Shared.Models;
using Services.Shared.UiModels;
using Shared.Enums;

namespace Services.Shared.UserServices
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClient<CurrentUser> _profileHttpClient;
        private readonly Logger<UserProfileService> _logger;

        public UserProfileService(
            AppDbContext dbContext,
            IUnitOfWork unitOfWork,
            IHttpClient<CurrentUser> profileHttpClient)
        {
            _unitOfWork = unitOfWork;
            _profileHttpClient = profileHttpClient;
            _logger = new Logger<UserProfileService>(dbContext);
        }

        public async Task<UserProfileModel?> GetProfile(int userId)
        {
            try
            {
                var userEntity = await _unitOfWork.UserRepository.FirstOrDefaultByIdAsync(userId);

                if (userEntity == null)
                {
                    throw new Exception($"User with ID [{userId}] not found.");
                }

                return new UserProfileModel
                {
                    UserId = userEntity.Id,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    UserName = userEntity.UserName,
                    Email = userEntity.EmailAddress,
                    DateOfBirth = userEntity.DateOfBirth,
                    UserRole = userEntity.UserRole
                };
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    $"Error while loading user profile [{userId}]", LogMessageTypeEnum.Error, exception);

                throw;
            }
        }

        public async Task<UserProfileModel?> UpdateProfile(UserProfileModel profile)
        {
            try
            {
                var userEntity = await _unitOfWork.UserRepository
                    .FirstOrDefaultByIdAsync(profile.UserId, false, x => x.UserSettings);

                if (userEntity == null || userEntity.UserSettings == null)
                {
                    throw new Exception($"User with ID [{profile.UserId}] not found.");
                }


                UpdateUserEntity(userEntity, profile);

                await _unitOfWork.CommittChanges(profile.Email);

                if (!await _profileHttpClient.CheckApiAvailabilityAsync())
                {
                    return new UserProfileModel
                    {
                        UserId = userEntity.Id,
                        FirstName = userEntity.FirstName,
                        LastName = userEntity.LastName,
                        UserName = userEntity.UserName,
                        Email = userEntity.EmailAddress,
                        ProfileImage = userEntity.ProfileImage,
                        DateOfBirth = userEntity.DateOfBirth,
                        UserRole = userEntity.UserRole,

                    };
                }

                if (userEntity.UserSettings.IsAutoDataSyncEnabled)
                {
                    var requestModel = new CurrentUser
                    {
                        Id = userEntity.Id,
                        UserIdExternal = userEntity.UserIdExternal,
                        FirstName = userEntity.FirstName,
                        LastName = userEntity.LastName,
                        DateOfBirth = userEntity.DateOfBirth,
                        EmailAddress = userEntity.EmailAddress,
                        ProfileImage = userEntity.ProfileImage,
                        UserRole = userEntity.UserRole,
                    };

                    var result = await _profileHttpClient.PostAsync("userprofile/updateprofile", requestModel);

                    if (!result.Success)
                    {
                        await _logger.LogMessageAsync(
                            $"Could not sync profile data of user [{userEntity.Id}]",
                            LogMessageTypeEnum.Warning,
                            null);
                    }
                }

                userEntity = await _unitOfWork.UserRepository.FirstOrDefaultByIdAsync(profile.UserId, false);

                if (userEntity == null)
                {
                    throw new Exception($"User with ID [{profile.UserId}] not found after update.");
                }

                return new UserProfileModel
                {
                    UserId = userEntity.Id,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    UserName = userEntity.UserName,
                    Email = userEntity.EmailAddress,
                    ProfileImage = userEntity.ProfileImage,
                    DateOfBirth = userEntity.DateOfBirth,
                    UserRole = userEntity.UserRole,

                };

            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    $"Error while updating user profile [{profile.UserId}]", LogMessageTypeEnum.Error, exception);

            }

            return null;
        }

        public async Task<ChangePasswordResult> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            try
            {
                var userEntity = await _unitOfWork.UserRepository
                     .FirstOrDefaultByIdAsync(changePasswordModel.UserId, false, x => x.UserCredentials);


                if (userEntity == null || userEntity.UserCredentials == null)
                {
                    throw new Exception($"User credentials for User ID [{changePasswordModel.UserId}] not found.");
                }

                if (!PasswordHasher.VerifyPassword(changePasswordModel.CurrentPassword, userEntity.UserCredentials.PasswordHash))
                {
                    throw new Exception("Old password is incorrect.");
                }

                var newPasswordHash = PasswordHasher.HashPassword(changePasswordModel.NewPassword);

                if (!PasswordHasher.VerifyPassword(changePasswordModel.PasswordReplication, newPasswordHash))
                {
                    throw new Exception("New passwords are not match.");
                }

                userEntity.UserCredentials.PasswordHash = newPasswordHash;

                await _unitOfWork.CommittChanges(userEntity.EmailAddress);

                await _unitOfWork.UserSettingsRepository.FirstOrDefaultByIdAsync(userEntity.UserSettingsId);

                if(!await _profileHttpClient.CheckApiAvailabilityAsync())
                {
                    return new ChangePasswordResult
                    {
                        Success = true,
                    };
                }

                if (userEntity.UserSettings != null && userEntity.UserSettings.IsAutoDataSyncEnabled)
                {
                    var requestModel = new ChangePasswordRequest
                    {
                        IdExternal = userEntity.UserIdExternal,
                        CurrentPassword = changePasswordModel.CurrentPassword,
                        NewPassword = changePasswordModel.NewPassword,
                        PasswordReplication = changePasswordModel.PasswordReplication
                    };

                    var result = await _profileHttpClient.PostAsync("userprofile/updatepassword", requestModel);

                    if (!result.Success)
                    {
                        await _logger.LogMessageAsync(
                            $"Could not sync profile data of user [{userEntity.Id}]",
                            LogMessageTypeEnum.Warning,
                            null);
                    }
                }

                return new ChangePasswordResult
                {
                    Success = true,
                };
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    $"Error while changing password for user [{changePasswordModel.UserId}]",
                    LogMessageTypeEnum.Error, exception);

                return new ChangePasswordResult
                {
                    Success = false,
                    ErrorMessage = exception.Message,
                };
            }
        }

        private void UpdateUserEntity(UserEntity entity, UserProfileModel profile)
        {
            entity.FirstName = profile.FirstName;
            entity.LastName = profile.LastName;
            entity.EmailAddress = profile.Email;
            entity.DateOfBirth = profile.DateOfBirth;
            
            if (profile.ProfileImage != null)
            {
                entity.ProfileImage = profile.ProfileImage;
            }
        }
    }
}

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
        private readonly Logger<UserProfileService> _logger;

        public UserProfileService(AppDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = new Logger<UserProfileService>(dbContext);
        }

        public async Task<UserProfileModel?> GetProfile(int userId)
        {
            try
            {
                var userEntity = await _unitOfWork.UserRepository.FirstOrDefaultByIdAsync(userId);

                if(userEntity == null)
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

                if(userEntity == null || userEntity.UserSettings == null)
                {
                    throw new Exception($"User with ID [{profile.UserId}] not found.");
                }


                UpdateUserEntity(userEntity, profile);

                await _unitOfWork.CommittChanges(profile.Email);

                if (userEntity.UserSettings.IsAutoDataSyncEnabled)
                {
                    // TODO : Trigger data sync process
                }

                userEntity = await _unitOfWork.UserRepository.FirstOrDefaultByIdAsync(profile.UserId, false);

                if(userEntity == null)
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

                if(!PasswordHasher.VerifyPassword(changePasswordModel.CurrentPassword, userEntity.UserCredentials.PasswordHash))
                {
                    throw new Exception("Old password is incorrect.");
                }

                var newPasswordHash = PasswordHasher.HashPassword(changePasswordModel.NewPassword);

                if(!PasswordHasher.VerifyPassword(changePasswordModel.PasswordReplication, newPasswordHash))
                {
                    throw new Exception("New passwords are not match.");
                }

                userEntity.UserCredentials.PasswordHash = newPasswordHash;
                
                await _unitOfWork.CommittChanges(userEntity.EmailAddress);

                await _unitOfWork.UserSettingsRepository.FirstOrDefaultByIdAsync(userEntity.UserSettingsId);

                if(userEntity.UserSettings != null && userEntity.UserSettings.IsAutoDataSyncEnabled)
                {
                    // TODO : Trigger data sync process
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
            entity.ProfileImage = profile.ProfileImage;
        }
    }
}

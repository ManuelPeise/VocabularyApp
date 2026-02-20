using Data.Accessor.Interfaces;
using Data.Database.Entities.User;
using Logic.Shared;
using Logic.Shared.Interfaces;
using Shared.Enums;
using Shared.Models.Authentication;
using Shared.Models.User;

namespace Logic.Administration
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(
            ILogger<UserProfileService> logger,
            IUnitOfWork unitOfWork,
            IHttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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
                if (string.IsNullOrEmpty(profile.Email))
                {
                    throw new Exception("Email address cannot be empty.");
                }

                var userEntity = await _unitOfWork.UserRepository
                    .FirstOrDefaultByIdAsync(profile.UserId, false, x => x.UserSettings);

                if (userEntity == null || userEntity.UserSettings == null)
                {
                    throw new Exception($"User with ID [{profile.UserId}] not found.");
                }

                UpdateUserEntity(userEntity, profile);

                await _unitOfWork.SaveChangesAsync(profile.Email);


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

                await _unitOfWork.SaveChangesAsync(userEntity.EmailAddress);

                await _unitOfWork.UserSettingsRepository.FirstOrDefaultByIdAsync(userEntity.UserSettingsId);

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

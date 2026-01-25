using Data.Database;
using Data.Database.Entities.User;
using Services.Shared.Interfaces;
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

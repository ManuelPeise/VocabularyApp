using Data.Database.Entities;
using Logic.Administration.Interfaces;
using Logic.Shared;
using Logic.Shared.Interfaces;
using Shared.Enums;
using Shared.Models.UserAdministration;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Logic.Administration
{
    public class UserAdministration : ALogicBase, IUserAdministration
    {
        private readonly IAdministrationUnitOfWork _administrationUnitOfWork;

        public UserAdministration(IAdministrationUnitOfWork administrationUnitOfWork) :
            base(administrationUnitOfWork.LogRepository, administrationUnitOfWork.CommittChanges)
        {
            _administrationUnitOfWork = administrationUnitOfWork;
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

                var salt = Guid.NewGuid().ToString();
                var userIdExternal = Guid.NewGuid();

                var entity = new UserEntity
                {
                    UserIdExternal = userIdExternal,
                    FirstName = registrationRequestModel.FirstName,
                    LastName = registrationRequestModel.LastName,
                    UserName = registrationRequestModel.UserName,
                    DateOfBirth = registrationRequestModel.DateOfBirth,
                    UserRole = UserRoleEnum.User,
                    UserCredentials = new UserCredentialsEntity
                    {
                        Salt = salt,
                        PasswordHash = GetHashedPassword(registrationRequestModel.Password, salt)
                    },
                };

                await _administrationUnitOfWork.UserRepository.AddAsync(entity, x => x.UserName == entity.UserName && x.UserIdExternal == userIdExternal);

                await _administrationUnitOfWork.CommittChanges("System");


                return new UserRegistrationResult { Result = true };
            }
            catch (Exception exception)
            {
                await LogMessageAsync(new LogMessageEntity
                {
                    Message = "Error occurred while creating user profile.",
                    ExeptionMessage = exception.Message,
                    Stacktrace = exception.StackTrace,
                    Module = nameof(UserAdministration),
                    LogLevel = LogLevelEnum.Error,
                });

                return new UserRegistrationResult { Result = false };
            }
        }

        public async Task<UserProfile?> LoadUserProfileAsync(int userId)
        {
            try
            {
                var userEntity = await _administrationUnitOfWork.UserRepository.FirstOrDefaultByIdAsync(userId, false);

                if (userEntity == null)
                {
                    throw new Exception($"Could not load user profile [{userId}]");
                }

                return new UserProfile
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
                await LogMessageAsync(new LogMessageEntity
                {
                    Message = $"Could not load user profile [{userId}]",
                    ExeptionMessage = exception.Message,
                    Stacktrace = exception.StackTrace,
                    Module = nameof(UserAdministration),
                    LogLevel = LogLevelEnum.Error
                });

                return null;
            }
        }

        public async Task<UserProfile?> UpdateUserProfile(UserProfile profile)
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
                userEntity.UserName = profile.UserName;
                userEntity.DateOfBirth = profile.DateOfBirth;
                userEntity.ProfileImage = profile.ProfileImage;
                userEntity.UpdatedAt = DateTime.UtcNow;
                userEntity.UpdatedBy = profile.UserName;

                await _administrationUnitOfWork.CommittChanges(userEntity.UserName);

                return new UserProfile
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
                await LogMessageAsync(new LogMessageEntity
                {
                    Message = $"Could not update user profile image [{profile.UserId}]",
                    ExeptionMessage = exception.Message,
                    Stacktrace = exception.StackTrace,
                    Module = nameof(UserAdministration),
                    LogLevel = LogLevelEnum.Error
                });

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

                if(userEntity.UserCredentials.PasswordHash != GetHashedPassword(model.CurrentPassword, userEntity.UserCredentials.Salt))
                {
                    return new ChangePasswordResult
                    {
                        Success = false,
                        ErrorMessage = Resx.Core.LabelCurrentPasswordDoesNotMatch
                    };
                }

                var newSalt = Guid.NewGuid().ToString();

                if(GetHashedPassword(model.NewPassword, newSalt) != GetHashedPassword(model.PasswordReplication, newSalt))
                {
                    return new ChangePasswordResult
                    {
                        Success = false,
                        ErrorMessage = Resx.Core.LabelPasswordsDoesNotMatch
                    };
                }

                userEntity.UserCredentials.Salt = newSalt;
                userEntity.UserCredentials.PasswordHash = GetHashedPassword(model.NewPassword, newSalt);

                await _administrationUnitOfWork.CommittChanges(userEntity.UserName);

                return new ChangePasswordResult
                {
                    Success = true,
                    ErrorMessage = string.Empty
                };
            }
            catch (Exception exception)
            {
                await LogMessageAsync(new LogMessageEntity
                {
                    Message = $"Cannot find user entity to change password [{model.UserId}].",
                    ExeptionMessage = exception.Message,
                    Stacktrace = exception.StackTrace,
                    Module = nameof(UserAdministration),
                    LogLevel = LogLevelEnum.Error
                });

                return new ChangePasswordResult
                {
                    Success = false,
                    ErrorMessage = Resx.Core.LabelChangePasswordFaild
                };
            }
        }

        private async Task<bool> IsValidRegistrationRequestModel(UserRegistrationRequestModel registrationRequestModel)
        {
            var existingUser = await _administrationUnitOfWork.UserRepository.FirstOrDefaultAsync(x => x.UserName == registrationRequestModel.UserName);

            if (existingUser != null)
            {
                return false;
            }

            return string.IsNullOrEmpty(registrationRequestModel.FirstName) ||
                   string.IsNullOrEmpty(registrationRequestModel.LastName) ||
                   string.IsNullOrEmpty(registrationRequestModel.UserName) ||
                   string.IsNullOrEmpty(registrationRequestModel.Password) &&
                   registrationRequestModel.Password.Length > 6 ? false : true;
        }
    }
}

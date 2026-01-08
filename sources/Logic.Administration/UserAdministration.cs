using Data.Database.Entities;
using Logic.Administration.Interfaces;
using Logic.Shared;
using Logic.Shared.Interfaces;
using Shared.Enums;
using Shared.Models.UserAdministration;

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

using Logic.Administration.Interfaces;
using Logic.Shared;
using Logic.Shared.Interfaces;
using Shared.Models.Authentication;
using System.Diagnostics;

namespace Logic.Administration
{
    public class UserAuthentication : ALogicBase, IUserAuthentication
    {
        private readonly IAdministrationUnitOfWork _administrationUnitOfWork;

        public UserAuthentication(IAdministrationUnitOfWork administrationUnitOfWork) : base(administrationUnitOfWork.LogRepository, administrationUnitOfWork.CommittChanges)
        {
            _administrationUnitOfWork = administrationUnitOfWork;
        }

        public async Task<AuthenticationResult> AuthenticateUser(AuthenticationRequestModel authenticationRequestModel)
        {
            try
            {
                if (string.IsNullOrEmpty(authenticationRequestModel.UserName) || string.IsNullOrEmpty(authenticationRequestModel.Password))
                {
                    throw new ArgumentException("Username and password must be provided.");
                }

                var user = await _administrationUnitOfWork.UserRepository
                    .FirstOrDefaultAsync(x => x.UserName == authenticationRequestModel.UserName, false, entity => entity.UserCredentials);

                if (user == null ||
                    user.UserCredentials == null ||
                    user.UserCredentials.PasswordHash != GetHashedPassword(authenticationRequestModel.Password, user.UserCredentials.Salt))
                {
                    throw new UnauthorizedAccessException("Invalid username or password.");
                }


                return new AuthenticationResult
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserRole = user.UserRole,
                    ProfileImage = user.ProfileImage,
                    IsAuthenticated = true
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during user authentication: {ex.Message}");

                return new AuthenticationResult
                {
                    IsAuthenticated = false
                };
            }
        }
    }
}

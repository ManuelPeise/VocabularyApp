using Data.Database;
using Data.Database.Entities.User;
using Logic.Shared;
using Logic.Shared.Interfaces;
using Logic.Shared.Models;
using Services.Shared.Interfaces;
using Services.Shared.UiModels;
using Shared.Enums;
using Shared.Interfaces;
using Shared.Models.Authentication;
using System.ComponentModel;

namespace Services.Shared
{
    public class CurrentUserService : ICurrentUserService, INotifyPropertyChanged
    {
        private readonly ISecureStorageHandler _secureStorageHandler;
        private readonly IHttpClient<CurrentUser> _currentUserClient;
        private readonly IHttpClient<AuthenticationResult> _authenticationClient;
        private readonly IAdministrationUnitOfWork _administrationUnitOfWork;
        private readonly Logger<CurrentUserService> _logger;

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        private UserData? _userData = null;
        public UserData? UserData
        {
            get => _userData;
            set
            {
                if (_userData != value)
                {
                    _userData = value;
                    OnPropertyChanged(nameof(UserData));
                }
            }
        }

        public CurrentUserService(
            AppDbContext dbContext,
            ISecureStorageHandler secureStorageHandler,
            IHttpClient<CurrentUser> currentUserClient,
            IHttpClient<AuthenticationResult> authenticationClient,
            IAdministrationUnitOfWork administrationUnitOfWork)
        {
            _secureStorageHandler = secureStorageHandler;
            _currentUserClient = currentUserClient;
            _authenticationClient = authenticationClient;
            _administrationUnitOfWork = administrationUnitOfWork;
            _logger = new Logger<CurrentUserService>(dbContext);
        }

        public async Task<bool> AuthenticateUser(AuthenticationRequestModel authData)
        {
            try
            {
                var localUser = await LoadUserFromDatabase(authData.Email);

                if (await TryAuthenticateLocalUser(authData, localUser))
                {
                    return true;
                }

                return await AuthenticateRemoteUser(authData);
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Error during user authentication", LogMessageTypeEnum.Error, exception);
                
                return false;
            }
        }

        private async Task<bool> TryAuthenticateLocalUser(AuthenticationRequestModel authData, CurrentUser? localUser)
        {
            if (localUser?.UserCredentials == null || 
                localUser.UserSettings == null ||
                string.IsNullOrEmpty(localUser.UserCredentials.RefreshToken))
            {
                return false;
            }

            if (!IsUserAuthenticated(authData, localUser.UserCredentials.PasswordHash))
            {
                return false;
            }

            await SetUserData(localUser);
            await _secureStorageHandler.SetValue(StorageKeys.RefreshTokenKey, localUser.UserCredentials.RefreshToken);

            if (await _currentUserClient.IsApiAvailableAsync && !localUser.UserSettings.IsAutoDataSyncEnabled)
            {
                return await SyncWithRemoteUser(localUser);
            }

            return true;
        }

        public async Task SignOutAsync()
        {
            try
            {
                _secureStorageHandler.RemoveValue(StorageKeys.UserDataKey);
                _secureStorageHandler.RemoveValue(StorageKeys.AccessTokenKey);
                _secureStorageHandler.RemoveValue(StorageKeys.RefreshTokenKey);

                UserData = null;
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Error during logout", LogMessageTypeEnum.Error, exception);
            }
        }

        private async Task<bool> SyncWithRemoteUser(CurrentUser localUser)
        {
            var remoteUserResponse = await _currentUserClient.GetAsync("api/userservice/getcurrentuser", null);

            if (remoteUserResponse.Success && remoteUserResponse.ResponseData != null)
            {
                await UpdateRemoteUserData(localUser, remoteUserResponse.ResponseData);

                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> AuthenticateRemoteUser(AuthenticationRequestModel authData)
        {
            if (!await _currentUserClient.IsApiAvailableAsync)
            {
                return false;
            }

            var signInResult = await _authenticationClient.PostAsync("userauthentication/authenticateuser", authData, null);

            if (!signInResult.Success ||
                signInResult.ResponseData == null ||
                string.IsNullOrEmpty(signInResult.ResponseData.AccessToken) ||
                string.IsNullOrEmpty(signInResult.ResponseData.RefreshToken))
            {
                return false;
            }

            await _secureStorageHandler.SetValue(StorageKeys.AccessTokenKey, signInResult.ResponseData.AccessToken);
            await _secureStorageHandler.SetValue(StorageKeys.RefreshTokenKey, signInResult.ResponseData.RefreshToken);

            var remoteUserResponse = await _currentUserClient.GetAsync("api/userservice/getcurrentuser", null);

            if (remoteUserResponse.Success && remoteUserResponse.ResponseData != null)
            {
                await SetUserData(remoteUserResponse.ResponseData);
                await SaveUserDataLocal(remoteUserResponse.ResponseData);
            }

            return true;
        }
        
        private async Task SetUserData(CurrentUser user)
        {
            UserData = new UserData
            {
                UserId = user.Id,
                Email = user.EmailAddress,
                ProfileImage = user.ProfileImage,
            };

            await _secureStorageHandler.SetValue(StorageKeys.UserDataKey, UserData);
        }

        private async Task<CurrentUser?> LoadUserFromDatabase(string email)
        {
            var user = await _administrationUnitOfWork.UserRepository
                           .FirstOrDefaultAsync(x => x.EmailAddress == email, false);

            if (user == null)
            {
                return null;
            }

            user.UserCredentials = await _administrationUnitOfWork.UserCredentialsRepository.FirstOrDefaultByIdAsync(user.UserCredentialsId);
            user.UserSettings = await _administrationUnitOfWork.UserSettingsRepository.FirstOrDefaultByIdAsync(user.UserSettingsId);

            return (CurrentUser)user;
        }

        private async Task SaveUserDataLocal(CurrentUser? userData)
        {
            if (userData == null)
            {
                return;
            }

            var entity = (UserEntity)userData;
            entity.Id = 0;
            entity.UserCredentialsId = 0;
            entity.UserSettingsId = 0;

            await _administrationUnitOfWork.UserRepository.AddAsync(entity);
            await _administrationUnitOfWork.CommittChanges(entity.EmailAddress);
        }

        private async Task UpdateRemoteUserData(CurrentUser localUser, CurrentUser? remoteUser)
        {
            if (localUser.UpdatedAt > remoteUser?.UpdatedAt ||
                localUser?.UserCredentials?.UpdatedAt > remoteUser?.UserCredentials?.UpdatedAt ||
                localUser?.UserSettings?.UpdatedAt > remoteUser?.UserSettings?.UpdatedAt)
            {
                await _currentUserClient.PostAsync("api/userservice/updatecurrentuser", localUser);
            }
        }

        private bool IsUserAuthenticated(AuthenticationRequestModel model, string passwordHash)
        {
           return PasswordHasher.HashPassword(model.Password) == passwordHash;
        }
    }
}

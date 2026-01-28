using Data.Database;
using Data.Database.Entities.User;
using Services.Shared.Interfaces;
using Services.Shared.Models;
using Services.Shared.Models.User;
using Services.Shared.UiModels;
using Shared.Enums;
using System.ComponentModel;

namespace Services.Shared
{
    public class CurrentUserService : ICurrentUserService, INotifyPropertyChanged
    {
        private readonly ISecureStorageHandler _secureStorageHandler;
        private readonly IHttpClient<UserModel> _userClient;
        private readonly IHttpClient<AuthenticationResult> _authenticationClient;
        private readonly IHttpClient<UserSettingsModel> _settingsClient;
        private readonly IUnitOfWork _administrationUnitOfWork;
        private readonly Logger<CurrentUserService> _logger;

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        private UserData? _userData = new UserData();
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
            IHttpClient<UserModel> userClient,
            IHttpClient<AuthenticationResult> authenticationClient,
            IHttpClient<UserSettingsModel> settingsClient,
            IUnitOfWork administrationUnitOfWork)
        {
            _secureStorageHandler = secureStorageHandler;
            _userClient = userClient;
            _authenticationClient = authenticationClient;
            _settingsClient = settingsClient;
            _administrationUnitOfWork = administrationUnitOfWork;
            _logger = new Logger<CurrentUserService>(dbContext);

            _ = Task.Run(async () => await Initialize());
        }

        private async Task Initialize()
        {
            UserData = await _secureStorageHandler.GetValue<UserData>(StorageKeys.UserDataKey) ?? null;
        }

        public async Task<bool> AuthenticateUser(AuthenticationRequestModel authData)
        {
            try
            {
                var localUser = await LoadUserFromDatabase(authData.Email);

                if (localUser != null)
                {
                    localUser = await TryAuthenticateLocalUser(authData, localUser);
                }

                if (localUser != null && localUser.UserCredentials?.RefreshToken != null)
                {
                    await _secureStorageHandler.SetValue(StorageKeys.RefreshTokenKey, localUser.UserCredentials.RefreshToken);
                    await SetUserData(localUser);

                    return true;
                }

                var remoteUser = await AuthenticateRemoteUser(authData);

                if (remoteUser == null || remoteUser?.UserCredentials?.RefreshToken == null)
                {
                    return false;
                }

                await SaveUserDataLocal(remoteUser);

                var savedLocalUser = await LoadUserFromDatabase(authData.Email);

                if (savedLocalUser == null || savedLocalUser.UserCredentials?.RefreshToken == null)
                {
                    await _logger.LogMessageAsync(
                        "Failed to reload user from database after saving",
                        LogMessageTypeEnum.Error);
                    return false;
                }

                await _secureStorageHandler.SetValue(StorageKeys.RefreshTokenKey, savedLocalUser.UserCredentials.RefreshToken);
                await SetUserData(savedLocalUser);

                return true;
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Error during user authentication", LogMessageTypeEnum.Error, exception);

                return false;
            }
        }

        public async Task SignOutAsync()
        {
            try
            {
                _secureStorageHandler.RemoveValue(StorageKeys.UserDataKey);
                _secureStorageHandler.RemoveValue(StorageKeys.RefreshTokenKey);

                UserData = null;
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Error during logout", LogMessageTypeEnum.Error, exception);
            }
        }

        public async Task<UserSettingsModel> GetCurrentUserSettings(int userId)
        {
            try
            {
                var userEntity = await _administrationUnitOfWork.UserRepository
                    .FirstOrDefaultAsync(x => x.Id == userId, false, x => x.UserSettings);

                if (userEntity?.UserSettings == null)
                {
                    throw new Exception("User settings not found.");
                }

                return new UserSettingsModel
                {
                    UserId = userId,
                    Culture = userEntity.UserSettings.Culture,
                    IsAutoDataSyncEnabled = userEntity.UserSettings.IsAutoDataSyncEnabled,
                    UseLocalDataStore = userEntity.UserSettings.UseLocalDataStore
                };

            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Error retrieving current user settings", LogMessageTypeEnum.Error, exception);

                return new UserSettingsModel
                {
                    Culture = CultureEnum.English,
                    IsAutoDataSyncEnabled = false,
                    UseLocalDataStore = false
                };
            }
        }

        public async Task UpdateUserSettings(UserSettingsModel model)
        {
            try
            {
                var userEntity = await _administrationUnitOfWork.UserRepository
                   .FirstOrDefaultAsync(x => x.Id == model.UserId, false, x => x.UserSettings);

                if (userEntity?.UserSettings == null)
                {
                    throw new Exception($"Could not find settings of user [{model.UserId}]");
                }

                var settings = userEntity.UserSettings;

                settings.Culture = model.Culture;
                settings.IsAutoDataSyncEnabled = model.IsAutoDataSyncEnabled;
                settings.UseLocalDataStore = model.UseLocalDataStore;

                await _administrationUnitOfWork.CommittChanges(userEntity.EmailAddress);

                if (await _settingsClient.IsApiAvailableAsync)
                {
                    var requestModel = new UserSettingsUpdateRequest
                    {
                        Culture = model.Culture,
                        IsAutoDataSyncEnabled = model.IsAutoDataSyncEnabled,
                        UseLocalDataStore = model.UseLocalDataStore,
                        UpdatedAt = settings.UpdatedAt,
                        UpdatedBy = settings.UpdatedBy
                    };

                    await _settingsClient.PostAsync("userprofile/updatesettings", requestModel);
                }
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync($"Could not update settings of user [{model.UserId}]", LogMessageTypeEnum.Error, exception);
            }
        }

        private async Task<UserModel?> TryAuthenticateLocalUser(AuthenticationRequestModel authData, UserModel? localUser)
        {
            if (localUser?.UserCredentials == null ||
                localUser.UserSettings == null ||
                string.IsNullOrEmpty(localUser.UserCredentials.RefreshToken))
            {
                return null;
            }

            if (!IsUserAuthenticated(authData, localUser.UserCredentials.PasswordHash))
            {
                return null;
            }

            if (await _userClient.IsApiAvailableAsync && !localUser.UserSettings.IsAutoDataSyncEnabled)
            {
                return await SyncWithRemoteUser(localUser);
            }

            return null;
        }

        private async Task<UserModel?> SyncWithRemoteUser(UserModel localUser)
        {
            var remoteUserResponse = await _userClient.GetAsync("userservice/getcurrentuser", null);

            if (remoteUserResponse.Success && remoteUserResponse.ResponseData != null)
            {
                await UpdateRemoteUserData(localUser, remoteUserResponse.ResponseData);

                return localUser;
            }

            return null;
        }

        private async Task<UserModel?> AuthenticateRemoteUser(AuthenticationRequestModel authData)
        {
            if (!await _userClient.IsApiAvailableAsync)
            {
                return null;
            }

            var signInResult = await _authenticationClient.PostAsync("userauthentication/authenticateuser", authData, null);

            if (!signInResult.Success ||
                signInResult.ResponseData == null ||
                string.IsNullOrEmpty(signInResult.ResponseData.AccessToken) ||
                string.IsNullOrEmpty(signInResult.ResponseData.RefreshToken))
            {
                return null;
            }

            await _secureStorageHandler.SetValue(StorageKeys.AccessTokenKey, signInResult.ResponseData.AccessToken);
            await _secureStorageHandler.SetValue(StorageKeys.RefreshTokenKey, signInResult.ResponseData.RefreshToken);

            var response = await _userClient.GetAsync("userservice/getcurrentuser", null);

            if (response.Success)
            {
                return response.ResponseData;
            }

            return null;
        }

        private async Task SetUserData(UserModel user)
        {
            UserData = new UserData
            {
                UserId = user.Id,
                Email = user.EmailAddress,
                ProfileImage = user.ProfileImage,
            };

            await _secureStorageHandler.SetValue(StorageKeys.UserDataKey, UserData);
        }

        private async Task<UserModel?> LoadUserFromDatabase(string email)
        {
            var user = await _administrationUnitOfWork.UserRepository
                           .FirstOrDefaultAsync(x => x.EmailAddress == email, false);

            if (user == null)
            {
                return null;
            }

            user.UserCredentials = await _administrationUnitOfWork.UserCredentialsRepository.FirstOrDefaultByIdAsync(user.UserCredentialsId);
            user.UserSettings = await _administrationUnitOfWork.UserSettingsRepository.FirstOrDefaultByIdAsync(user.UserSettingsId);

            return new UserModel
            {
                Id = user.Id,
                UserIdExternal = user.UserIdExternal,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                EmailAddress = user.EmailAddress,
                ProfileImage = user.ProfileImage,
                UserRole = user.UserRole,
                UserCredentialsId = user.UserCredentialsId,
                UserCredentials = new UserCredentials
                {
                    Id = user.UserCredentials?.Id ?? 0,
                    PasswordHash = user.UserCredentials?.PasswordHash ?? string.Empty,
                    RefreshToken = user.UserCredentials?.RefreshToken ?? string.Empty,
                    CreatedAt = user.UserCredentials?.CreatedAt ?? DateTime.UtcNow,
                    CreatedBy = user.UserCredentials?.CreatedBy ?? "System",
                    UpdatedAt = user.UserCredentials?.UpdatedAt ?? DateTime.UtcNow,
                    UpdatedBy = user.UserCredentials?.UpdatedBy ?? "System"
                },
                UserSettingsId = user.UserSettingsId,
                UserSettings = new UserSettings
                {
                    Id = user.UserSettings?.Id ?? 0,
                    IsAutoDataSyncEnabled = user.UserSettings?.IsAutoDataSyncEnabled ?? false,
                    UseLocalDataStore = user.UserSettings?.UseLocalDataStore ?? false,
                    CreatedAt = user.UserSettings?.CreatedAt ?? DateTime.UtcNow,
                    CreatedBy = user.UserSettings?.CreatedBy ?? "System",
                    UpdatedAt = user.UserSettings?.UpdatedAt ?? DateTime.UtcNow,
                    UpdatedBy = user.UserSettings?.UpdatedBy ?? "System"
                },
                CreatedAt = user.CreatedAt,
                CreatedBy = user.CreatedBy,
                UpdatedAt = user.UpdatedAt,
                UpdatedBy = user.UpdatedBy
            };
        }

        private async Task SaveUserDataLocal(UserModel? userData)
        {
            if (userData == null)
            {
                return;
            }

            var entity = new UserEntity
            {
                UserIdExternal = userData.UserIdExternal,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                DateOfBirth = userData.DateOfBirth,
                EmailAddress = userData.EmailAddress,
                ProfileImage = userData.ProfileImage,
                UserRole = userData.UserRole,
                UserCredentials = new UserCredentialsEntity
                {
                    PasswordHash = userData.UserCredentials?.PasswordHash ?? string.Empty,
                    RefreshToken = userData.UserCredentials?.RefreshToken ?? string.Empty,
                    CreatedAt = userData.UserCredentials?.CreatedAt ?? DateTime.UtcNow,
                    CreatedBy = userData.UserCredentials?.CreatedBy ?? "System",
                    UpdatedAt = userData.UserCredentials?.UpdatedAt ?? DateTime.UtcNow,
                    UpdatedBy = userData.UserCredentials?.UpdatedBy ?? "System"
                },
                UserSettings = new UserSettingsEntity
                {
                    IsAutoDataSyncEnabled = userData.UserSettings?.IsAutoDataSyncEnabled ?? false,
                    UseLocalDataStore = userData.UserSettings?.UseLocalDataStore ?? false,
                    CreatedAt = userData.UserSettings?.CreatedAt ?? DateTime.UtcNow,
                    CreatedBy = userData.UserSettings?.CreatedBy ?? "System",
                    UpdatedAt = userData.UserSettings?.UpdatedAt ?? DateTime.UtcNow,
                    UpdatedBy = userData.UserSettings?.UpdatedBy ?? "System"
                },
                CreatedAt = userData.CreatedAt,
                CreatedBy = userData.CreatedBy,
                UpdatedAt = userData.UpdatedAt,
                UpdatedBy = userData.UpdatedBy
            };

            await _administrationUnitOfWork.UserRepository.AddAsync(entity);
            await _administrationUnitOfWork.CommittChanges(entity.EmailAddress);
        }

        private async Task UpdateRemoteUserData(UserModel localUser, UserModel? remoteUser)
        {
            if (localUser.UpdatedAt > remoteUser?.UpdatedAt ||
                localUser?.UserCredentials?.UpdatedAt > remoteUser?.UserCredentials?.UpdatedAt ||
                localUser?.UserSettings?.UpdatedAt > remoteUser?.UserSettings?.UpdatedAt)
            {
                await _userClient.PostAsync("userservice/updatecurrentuser", localUser);
            }
        }

        private bool IsUserAuthenticated(AuthenticationRequestModel model, string passwordHash)
        {
            return PasswordHasher.VerifyPassword(model.Password, passwordHash);
        }
    }
}

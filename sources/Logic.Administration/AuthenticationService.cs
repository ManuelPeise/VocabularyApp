using Data.Accessor.Interfaces;
using Data.Database.Entities.User;
using Logic.Shared;
using Logic.Shared.Interfaces;
using Newtonsoft.Json;
using Shared.Enums;
using Shared.Models.Authentication;
using Shared.Models.Sync;
using Shared.Models.User;

namespace Logic.Administration
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClient _httpClient;
        private readonly ISecureStorageHandler _secureStorageHandler;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            ILogger<AuthenticationService> logger,
            IUnitOfWork unitOfWork,
            IHttpClient httpClient,
            ISecureStorageHandler secureStorageHandler)
        {
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
            _secureStorageHandler = secureStorageHandler;
            _logger = logger;
        }

        public async Task<CurrentUser?> AuthenticateUser(AuthenticationRequestModel authRequest)
        {
            try
            {
                AuthenticationResult? result;

                if (string.IsNullOrEmpty(authRequest.Email) || string.IsNullOrEmpty(authRequest.Password))
                {
                    throw new Exception("Email and password must be provided.");
                }

                var userEntity = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.EmailAddress == authRequest.Email);

                if (userEntity != null)
                {
                    await _unitOfWork.UserCredentialsRepository.FirstOrDefaultByIdAsync(userEntity.UserCredentialsId);
                }

                if (userEntity == null || userEntity.UserCredentials == null)
                {
                    if (!await _httpClient.IsApiAvailableAsync)
                    {
                        throw new Exception("Api is not available.");
                    }

                    var response = await _httpClient.SendPostRequest("userauthentication/authenticateuser", null, authRequest);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(content))
                    {
                        throw new Exception("Authentication service returned an empty response.");
                    }

                    result = JsonConvert.DeserializeObject<AuthenticationResult>(content);

                    var userSync = await ProcessRemoteAuthenticationResult(result);

                    var userId = await StoreUserInDatabase(userSync);

                    if (userSync == null || userId == -1)
                    {
                        throw new Exception("User scnc data could not be null.");
                    }

                    var userData = new CurrentUser
                    {
                        UserId = userId,
                        Email = userSync.EmailAddress,
                        ProfileImage = userSync.ProfileImage,
                    };

                    return userData;
                }

                // Verify that the provided password matches the stored hash
                if (!PasswordHasher.VerifyPassword(authRequest.Password, userEntity.UserCredentials.PasswordHash))
                {
                    throw new Exception("Password does not match.");
                }

                return new CurrentUser
                {
                    UserId = userEntity.Id,
                    Email = userEntity.EmailAddress,
                    ProfileImage = userEntity.ProfileImage
                };
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    $"An error occurred while authenticating.",
                    LogMessageTypeEnum.Warning,
                    exception);
            }

            return null;
        }

        public async Task SignOutAsync(CurrentUser? userData)
        {
            try
            {
                _secureStorageHandler.RemoveValue(StorageKeys.UserDataKey);
                _secureStorageHandler.RemoveValue(StorageKeys.RefreshTokenKey);

                userData = null;
            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync(
                    "Error during logout", LogMessageTypeEnum.Error, exception);
            }
        }

        public async Task<CurrentUser?> GetCurrentUser()
        {
            return await _secureStorageHandler.GetValue<CurrentUser>(StorageKeys.UserDataKey) ?? null;
        }

        private async Task<UserDataSyncModel?> ProcessRemoteAuthenticationResult(AuthenticationResult? result)
        {
            UserDataSyncModel? userSync = null;

            if (result == null || string.IsNullOrEmpty(result.AccessToken) || string.IsNullOrEmpty(result.RefeshToken))
            {
                throw new Exception("Invalid authentication result received from remote service.");
            }

            await _secureStorageHandler.SetValue(StorageKeys.AccessTokenKey, result.AccessToken);
            await _secureStorageHandler.SetValue(StorageKeys.RefreshTokenKey, result.RefeshToken);

            var response = await _httpClient.SendGetRequest("syncronization/pulluserdata");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            userSync = JsonConvert.DeserializeObject<UserDataSyncModel>(content);

            return userSync;
        }

        private async Task<int> StoreUserInDatabase(UserDataSyncModel? userSync)
        {
            if (userSync == null)
            {
                return -1;
            }

            var userEntity = new UserEntity
            {
                IdExternal = userSync.IdExternal,
                FirstName = userSync.FirstName,
                LastName = userSync.LastName,
                EmailAddress = userSync.EmailAddress,
                DateOfBirth = userSync.DateOfBirth,
                ProfileImage = userSync.ProfileImage,
                UserRole = userSync.UserRole,
                UserCredentials = new UserCredentialsEntity
                {
                    IdExternal = userSync.IdExternal,
                    PasswordHash = userSync.UserCredentials?.PasswordHash ?? string.Empty,
                    ExpireDate = userSync.UserCredentials?.ExpireDate ?? DateTime.UtcNow.AddDays(30),
                    RefreshToken = userSync.UserCredentials?.RefreshToken ?? string.Empty,
                    IsDirty = false,
                    CreatedAt = userSync?.UserCredentials?.CreatedAt ?? DateTime.MinValue,
                    CreatedBy = userSync?.UserCredentials?.CreatedBy ?? string.Empty,
                    UpdatedAt = userSync?.UserCredentials?.UpdatedAt ?? DateTime.MinValue,
                    UpdatedBy = userSync?.UserCredentials?.UpdatedBy ?? string.Empty
                },
                UserSettings = new UserSettingsEntity
                {
                    IdExternal = userSync?.UserSettings?.IdExternal ?? Guid.Empty,
                    Culture = userSync?.UserSettings?.Culture ?? CultureEnum.English,
                    IsAutoDataSyncEnabled = userSync?.UserSettings?.IsAutoDataSyncEnabled ?? false,
                    IsDirty = false,
                    CreatedAt = userSync?.UserSettings?.CreatedAt ?? DateTime.MinValue,
                    CreatedBy = userSync?.UserSettings?.CreatedBy ?? string.Empty,
                    UpdatedAt = userSync?.UserSettings?.UpdatedAt ?? DateTime.MinValue,
                    UpdatedBy = userSync?.UserSettings?.UpdatedBy ?? string.Empty
                },
                CreatedAt = userSync?.CreatedAt ?? DateTime.MinValue,
                CreatedBy = userSync?.CreatedBy ?? string.Empty,
                UpdatedAt = userSync?.UpdatedAt ?? DateTime.MinValue,
                UpdatedBy = userSync?.UpdatedBy ?? string.Empty
            };

            await _unitOfWork.UserRepository.AddAsync(userEntity);

            await _unitOfWork.SaveChangesAsync("System");

            return userEntity.Id;
        }
    }
}

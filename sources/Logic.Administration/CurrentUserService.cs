using Data.Accessor;
using Data.Accessor.Interfaces;
using Logic.Shared.Interfaces;
using Shared.Enums;
using Shared.Models.Authentication;
using Shared.Models.User;
using System.ComponentModel;


namespace Logic.Administration
{
    public class CurrentUserService : ICurrentUserService, INotifyPropertyChanged
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<CurrentUserService> _logger;

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        private CurrentUser? _userData = new CurrentUser();
        public CurrentUser? UserData
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
            IUnitOfWork unitOfWork,
            ILogger<CurrentUserService> logger,
            IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _logger = logger;
            _unitOfWork = unitOfWork;

            _ = Task.Run(async () => await Initialize());
        }

        private async Task Initialize()
        {
            UserData = await _authenticationService.GetCurrentUser();
        }

        public async Task<bool> AuthenticateUser(AuthenticationRequestModel authData) => await _authenticationService.AuthenticateUser(authData, UserData);

        public async Task SignOutAsync() => await _authenticationService.SignOutAsync(UserData);



        public async Task<UserSettingsModel> GetCurrentUserSettings(int userId)
        {
            try
            {
                var userEntity = await _unitOfWork.UserRepository
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
                var userEntity = await _unitOfWork.UserRepository
                   .FirstOrDefaultAsync(x => x.Id == model.UserId, false, x => x.UserSettings);

                if (userEntity?.UserSettings == null)
                {
                    throw new Exception($"Could not find settings of user [{model.UserId}]");
                }

                var settings = userEntity.UserSettings;

                settings.Culture = model.Culture;
                settings.IsAutoDataSyncEnabled = model.IsAutoDataSyncEnabled;
                settings.UseLocalDataStore = model.UseLocalDataStore;

                await _unitOfWork.SaveChangesAsync(userEntity.EmailAddress);

            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync($"Could not update settings of user [{model.UserId}]", LogMessageTypeEnum.Error, exception);
            }
        }
    }
}

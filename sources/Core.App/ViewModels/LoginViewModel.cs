using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.App.Services.Interfaces;
using Logic.Administration.Interfaces;
using Shared.Models.Authentication;
using System.Diagnostics;
using System.Text.Json;

namespace Core.App.ViewModels
{
    public partial class LoginViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserAuthentication _authenticationService;
        private readonly ISecureStorageHandler _secureStorageHandler;

        [ObservableProperty]
        private AuthenticationRequestModel _authData = new AuthenticationRequestModel();
        [ObservableProperty]
        private string? _errorMessage;

        public LoginViewModel(
            IUserAuthentication authenticationService, 
            ICurrentUserService currentUserService,
            ISecureStorageHandler secureStorageHandler)
        {
            _authenticationService = authenticationService;
            _currentUserService = currentUserService;
            _secureStorageHandler = secureStorageHandler;
           
            Task.Run(async () => await InitializeAsync());
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("userName", out var userNameObj) && userNameObj is string userName)
            {
                AuthData.UserName = userName;
            }
        }

        private async Task InitializeAsync()
        {
            var authData = await _secureStorageHandler.GetAsync(StorageKeys.LoginDataKey);

            if (!string.IsNullOrEmpty(authData))
            {
                AuthData = JsonSerializer.Deserialize<AuthenticationRequestModel>(authData) ?? new AuthenticationRequestModel();
            }   
        }

        [RelayCommand]
        private async Task NavigateToStart()
        {
            await Shell.Current.GoToAsync("StartPage");
        }

        [RelayCommand]
        private async Task Login()
        {
            ErrorMessage = null;

            try
            {
                IsBusy = true;

                var result = await _authenticationService.AuthenticateUser(AuthData);

                if (result.IsAuthenticated)
                {
                    _currentUserService.SetCurrentUser(result);

                    if (AuthData.RememberMe)
                    {
                        await _secureStorageHandler.SetAsync(StorageKeys.LoginDataKey, JsonSerializer.Serialize(AuthData));
                    }
                    else
                    {
                        _secureStorageHandler.Remove(StorageKeys.LoginDataKey);
                    }

                    await _secureStorageHandler.SetAsync(StorageKeys.UserData, JsonSerializer.Serialize(result));
                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    ErrorMessage = Resx.Core.LabelLoginError;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Login failed: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

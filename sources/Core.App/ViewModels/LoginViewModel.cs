using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.Shared.Interfaces;
using Services.Shared;
using Services.Shared.Interfaces;
using Shared.Models.Authentication;
using System.Diagnostics;

namespace Core.App.ViewModels
{
    public partial class LoginViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ISecureStorageHandler _secureStorageHandler;

        [ObservableProperty]
        private AuthenticationRequestModel _authData = new AuthenticationRequestModel();
        [ObservableProperty]
        private string? _errorMessage;

        public LoginViewModel(
            ICurrentUserService currentUserService,
            ISecureStorageHandler secureStorageHandler)
        {
            _currentUserService = currentUserService;
            _secureStorageHandler = secureStorageHandler;
           
            Task.Run(async () => await InitializeAsync());
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("userName", out var userNameObj) && userNameObj is string email)
            {
                AuthData.Email = email;
            }

        }

        private async Task InitializeAsync()
        {
            var authData = await _secureStorageHandler.GetValue<AuthenticationRequestModel>(StorageKeys.LoginDataKey);

            AuthData = authData ?? new AuthenticationRequestModel();
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

                if (await _currentUserService.AuthenticateUser(AuthData))
                {
                    if (AuthData.RememberMe)
                    {
                        await _secureStorageHandler.SetValue(StorageKeys.LoginDataKey, AuthData);
                    }
                    else
                    {
                        _secureStorageHandler.RemoveValue(StorageKeys.LoginDataKey);
                    }

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

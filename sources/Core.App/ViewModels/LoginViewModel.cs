using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.Administration.Interfaces;
using Shared.Models.Authentication;
using System.Diagnostics;

namespace Core.App.ViewModels
{
    public partial class LoginViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserAuthentication _authenticationService;

        [ObservableProperty]
        private string _userName = string.Empty;
        [ObservableProperty]
        private string _password = string.Empty;
        [ObservableProperty]
        private bool _rememberMe = false;
        [ObservableProperty]
        private string? _errorMessage;

        public LoginViewModel(IUserAuthentication authenticationService, ICurrentUserService currentUserService)
        {
            _authenticationService = authenticationService;
            _currentUserService = currentUserService;
            IsBusy = false;

            Debug.WriteLine($"Is Busy: {IsBusy}");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("userName", out var userNameObj) && userNameObj is string userName)
            {
                UserName = userName;
            }
        }

        [RelayCommand]
        private async Task NavigateToStart()
        {
            await Shell.Current.GoToAsync("//StartPage");
        }

        [RelayCommand]
        private async Task Login()
        {
            ErrorMessage = null;

            try
            {
                IsBusy = true;

                var result = await _authenticationService.AuthenticateUser(new AuthenticationRequestModel
                {
                    UserName = UserName,
                    Password = Password
                });

                if (result.IsAuthenticated)
                {
                    _currentUserService.SetCurrentUser(result);
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

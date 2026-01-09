using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.App.Services.Interfaces;
using Logic.Administration.Interfaces;
using Shared.Models.Authentication;
using System.ComponentModel;

namespace Core.App.ViewModels
{
    public partial class PrivateViewModelBase : ViewModelBase
    {
        private AuthenticationResult? _authResult;
        private PropertyChangedEventHandler? _authenticationResultHandler;

        public AuthenticationResult? AuthResult
        {
            get => _authResult;
            set
            {
                if (_authResult != value)
                {
                    if (_authResult is INotifyPropertyChanged oldNotify && _authenticationResultHandler != null)
                    {
                        oldNotify.PropertyChanged -= _authenticationResultHandler;
                    }

                    _authResult = value;
                    OnPropertyChanged(nameof(AuthResult));

                    if (_authResult is INotifyPropertyChanged newNotify)
                    {
                        _authenticationResultHandler = AuthenticationResult_PropertyChanged;
                        newNotify.PropertyChanged += _authenticationResultHandler;
                    }
                }
            }
        }

        [ObservableProperty]
        private ICurrentUserService _currentUserService;
        [ObservableProperty]
        private ISecureStorageHandler _secureStorageHandler;

        public PrivateViewModelBase(ICurrentUserService currentUserService, ISecureStorageHandler secureStorageHandler)
        {
            CurrentUserService = currentUserService;
            AuthResult = CurrentUserService.AuthenticationResult;
            SecureStorageHandler = secureStorageHandler;

            if (CurrentUserService is INotifyPropertyChanged notify)
            {
                notify.PropertyChanged += CurrentUserService_PropertyChanged;
            }
        }

        [RelayCommand]
        private async Task HandleLogout()
        {
            CurrentUserService.SetCurrentUser(new AuthenticationResult
            {
                IsAuthenticated = false
            });
            
            SecureStorageHandler.Remove(StorageKeys.UserData);
            
            await Shell.Current.GoToAsync("LoginPage");
        }

        private void AuthenticationResult_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentUserService.AuthenticationResult.UserName))
            {
                AuthResult = CurrentUserService.AuthenticationResult;
            }
        }

        private void CurrentUserService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentUserService.AuthenticationResult))
            {
                AuthResult = CurrentUserService.AuthenticationResult;
            }
        }
    }
}

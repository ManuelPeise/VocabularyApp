using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.Shared.Interfaces;
using Services.Shared.UiModels;
using System.ComponentModel;

namespace Core.App.ViewModels
{
    public partial class PrivateViewModelBase : ViewModelBase
    {
        private UserData? _userData;
        private PropertyChangedEventHandler? _authenticationResultHandler;

        public UserData? UserData
        {
            get => _userData;
            set
            {
                if (_userData != value)
                {
                    if (_userData is INotifyPropertyChanged oldNotify && _authenticationResultHandler != null)
                    {
                        oldNotify.PropertyChanged -= _authenticationResultHandler;
                    }

                    _userData = value;
                    OnPropertyChanged(nameof(UserData));

                    if (_userData is INotifyPropertyChanged newNotify)
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
      
        public PrivateViewModelBase(
            ICurrentUserService currentUserService, 
            ISecureStorageHandler secureStorageHandler, 
            ILocalizationResourceManager localizationResourceManager) : base(secureStorageHandler, localizationResourceManager)
        {
            CurrentUserService = currentUserService;
            UserData = CurrentUserService.UserData ?? new UserData();
            SecureStorageHandler = secureStorageHandler;
            
            if (CurrentUserService is INotifyPropertyChanged notify)
            {
                notify.PropertyChanged += CurrentUserService_PropertyChanged;
            }
        }

        protected async Task ToggleLanguage(DropdownItem selectedLanguage)
        {
            await ChangeLanguage(selectedLanguage);
        }

        [RelayCommand]
        private async Task HandleLogout()
        {
            await CurrentUserService.SignOutAsync();
            
            await Shell.Current.GoToAsync("LoginPage");
        }

        [RelayCommand]
        private async Task NavigateToUserProfile() 
        {
            await Shell.Current.GoToAsync("UserProfilePage");
        }

        private void AuthenticationResult_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentUserService.UserData.Email))
            {
                UserData = CurrentUserService.UserData;
            }
        }

        private void CurrentUserService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentUserService.UserData))
            {
                UserData = CurrentUserService.UserData;
            }
        }
    }
}

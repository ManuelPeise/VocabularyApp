using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.Shared.Interfaces;
using Shared.Models.UiModels;
using Shared.Models.User;
using System.ComponentModel;

namespace Core.App.ViewModels
{
    public partial class PrivateViewModelBase : ViewModelBase
    {
        // Expose current user data directly from the user service so all view models
        // (including AppShell) always see the same instance and updates.
        public CurrentUser? UserData
        {
            get => CurrentUserService.UserData;
            set
            {
                if (!ReferenceEquals(CurrentUserService.UserData, value))
                {
                    CurrentUserService.UserData = value;
                    OnPropertyChanged(nameof(UserData));
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

        private void CurrentUserService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentUserService.UserData))
            {
                // Bubble up changes from the service so bindings to UserData.Email update
                OnPropertyChanged(nameof(UserData));
            }
        }
    }
}

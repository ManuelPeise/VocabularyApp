using CommunityToolkit.Mvvm.Input;
using Logic.Shared.Interfaces;

namespace Core.App.ViewModels
{
    public partial class StartPageViewModel : ViewModelBase
    {
       
        public StartPageViewModel(
            ISecureStorageHandler secureStorageHandler,
            ILocalizationResourceManager localizationResourceManager) : base(secureStorageHandler, localizationResourceManager) { }

        [RelayCommand]
        private async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("LoginPage");
        }


        [RelayCommand]
        private async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }
    }
}

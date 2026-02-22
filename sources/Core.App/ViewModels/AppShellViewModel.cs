using CommunityToolkit.Mvvm.Input;
using Logic.Shared.Interfaces;

namespace Core.App.ViewModels
{
    public partial class AppShellViewModel : PrivateViewModelBase
    {
        public AppShellViewModel(
            ICurrentUserService currentUserService, 
            ISecureStorageHandler secureStorageHandler,
            ILocalizationResourceManager localizationResourceManager) :
            base(currentUserService, secureStorageHandler, localizationResourceManager)
        {
        }

        [RelayCommand]
        private async Task Logout()
        {
            await CurrentUserService.SignOutAsync();
        }
    }
}

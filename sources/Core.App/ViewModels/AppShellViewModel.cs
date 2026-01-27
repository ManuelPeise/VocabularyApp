using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.Shared.Interfaces;
using Services.Shared.UiModels;

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

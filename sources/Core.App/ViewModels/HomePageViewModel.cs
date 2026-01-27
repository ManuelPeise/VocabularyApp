using CommunityToolkit.Mvvm.Input;
using Services.Shared.Interfaces;

namespace Core.App.ViewModels
{
    public partial class HomePageViewModel : PrivateViewModelBase
    {

        public HomePageViewModel(
            ICurrentUserService currentUserService,
            ISecureStorageHandler secureStorageHandler,
            ILocalizationResourceManager localizationResourceManager) : base(currentUserService, secureStorageHandler, localizationResourceManager)
        {

        }

        [RelayCommand]
        private void ToggleIsLoading()
        {
            IsBusy = !IsBusy;
        }
    }
}

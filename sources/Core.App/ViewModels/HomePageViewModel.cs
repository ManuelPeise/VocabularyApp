using CommunityToolkit.Mvvm.Input;
using Logic.Shared.Interfaces;
using Services.Shared.Interfaces;



namespace Core.App.ViewModels
{
    public partial class HomePageViewModel : PrivateViewModelBase
    {
     
        public HomePageViewModel(ICurrentUserService currentUserService, ISecureStorageHandler secureStorageHandler) :
            base(currentUserService, secureStorageHandler)
        {
           
        }

        [RelayCommand]
        private void ToggleIsLoading()
        {
            IsBusy = !IsBusy;
        }
    }
}

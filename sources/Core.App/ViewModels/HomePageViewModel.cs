using CommunityToolkit.Mvvm.Input;
using Core.App.Services.Interfaces;
using Logic.Administration.Interfaces;
using Shared.Models.Authentication;



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

using CommunityToolkit.Mvvm.Input;

namespace Core.App.ViewModels
{
    public partial class StartPageViewModel : ViewModelBase
    {
       
        public StartPageViewModel()
        {

        }

        [RelayCommand]
        private async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }


        [RelayCommand]
        private async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}

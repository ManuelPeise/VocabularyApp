using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.Administration.Interfaces;


namespace Core.App.ViewModels
{
    public partial class HomePageViewModel : ViewModelBase
    {
        private const string Greeting = "Hallo {User}!";
        private readonly ICurrentUserService _currentUserService;

        [ObservableProperty]
        private string _greetingText;

        public HomePageViewModel(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            GreetingText = Greeting.Replace("{User}", _currentUserService.UserName);
        }

        [RelayCommand]
        private void ToggleIsLoading()
        {
            IsBusy = !IsBusy;
        }
    }
}

using Core.App.ViewModels;
using Services.Shared;
using Services.Shared.Interfaces;
using Services.Shared.UiModels;

namespace Core.App
{
    public partial class App : Application
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ISecureStorageHandler _secureStorageHandler;
        
        public App(ICurrentUserService currentUserService, ISecureStorageHandler secureStorageHandler)
        {
            InitializeComponent();
            _currentUserService = currentUserService;
            _secureStorageHandler = secureStorageHandler;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell(new AppShellViewModel(_currentUserService, _secureStorageHandler)));
        }

        protected override async void OnStart()
        {
            var userData = await _secureStorageHandler.GetValue<UserData>(StorageKeys.UserDataKey);

            if (userData != null) 
            {
                _currentUserService.UserData = userData;

                return;
            }

            await Shell.Current.GoToAsync("StartPage");
        }
    }
}
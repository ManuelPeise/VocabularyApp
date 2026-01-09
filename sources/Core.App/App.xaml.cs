using Core.App.Services.Interfaces;
using Core.App.ViewModels;
using Logic.Administration.Interfaces;
using Shared.Models.Authentication;
using System.Text.Json;

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
            AuthenticationResult? authResult = null;

            var authResultJson = await _secureStorageHandler.GetAsync(StorageKeys.UserData);

            if(!string.IsNullOrEmpty(authResultJson))
            {
                authResult = JsonSerializer.Deserialize<AuthenticationResult>(authResultJson);
            }


            if(authResult != null && authResult.IsAuthenticated)
            {
                _currentUserService.SetCurrentUser(authResult);
            } 
            else
            {
                await Shell.Current.GoToAsync("StartPage");
            }
        }
    }
}
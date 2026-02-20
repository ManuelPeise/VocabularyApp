using Core.App.ViewModels;
using Services.Shared;
using Services.Shared.Interfaces;
using Services.Shared.UiModels;
using Shared.Enums;
using Shared.Models.User;
using System.Globalization;

namespace Core.App
{
    public partial class App : Application
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ISecureStorageHandler _secureStorageHandler;
        private readonly ILocalizationResourceManager _localizationResourceManager;
        public App(ICurrentUserService currentUserService, ISecureStorageHandler secureStorageHandler, ILocalizationResourceManager localizationResourceManager)
        {
            InitializeComponent();
            _currentUserService = currentUserService;
            _secureStorageHandler = secureStorageHandler;
            _localizationResourceManager = localizationResourceManager;

            _ = InitializeLanguage();
           
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell(new AppShellViewModel(_currentUserService, _secureStorageHandler, _localizationResourceManager)));
        }

        protected override async void OnStart()
        {
            var userData = await _secureStorageHandler.GetValue<CurrentUser>(StorageKeys.UserDataKey);

            if (userData != null)
            {
                _currentUserService.UserData = userData;

                return;
            }

            await Shell.Current.GoToAsync("StartPage");
        }

        private async Task InitializeLanguage()
        {
            var cultureString = "en-Us";
            var cultureKey = CultureEnum.English;
            var savedLanguage = await _secureStorageHandler.GetValue<string>(StorageKeys.LanguagePreferenceKey);

            if (!string.IsNullOrEmpty(savedLanguage))
            {
                cultureKey = (CultureEnum)Enum.Parse(typeof(CultureEnum), savedLanguage);
            }

            switch (cultureKey)
            {
                case CultureEnum.English:
                    cultureString = "en-US";
                    break;
                case CultureEnum.German:
                    cultureString = "de-DE";
                    break;
                default:
                    cultureString = "en-US";
                    break;

            }


            var culture = new CultureInfo(cultureString);

            _localizationResourceManager.SetCulture(culture);
        }
    }
}

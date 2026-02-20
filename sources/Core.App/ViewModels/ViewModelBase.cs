
using CommunityToolkit.Mvvm.ComponentModel;
using Logic.Shared.Interfaces;
using Services.Shared.Interfaces;
using Services.Shared.UiModels;
using Shared.Enums;
using System.Globalization;

namespace Core.App.ViewModels
{
    public partial class ViewModelBase : ObservableObject
    {
        private readonly ISecureStorageHandler _secureStorageHandler;
        
        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private ILocalizationResourceManager _localizer;
        
        public ViewModelBase(ISecureStorageHandler secureStorageHandler, ILocalizationResourceManager localizationResourceManager)
        {
            _secureStorageHandler = secureStorageHandler;
            _localizer = localizationResourceManager;
        }

        protected async Task ChangeLanguage(DropdownItem selectedLanguage)
        {
            try
            {
                var cultureCode = "en-US";
                IsBusy = true;

                switch (selectedLanguage.Id)
                {
                    case (int)CultureEnum.English:
                        cultureCode = "en-US";
                        break;
                    case (int)CultureEnum.German:
                        cultureCode = "de-DE";
                        break;
                    default:
                        cultureCode = "en-US";
                        break;
                }

                // Set the culture for the application
                var culture = new CultureInfo(cultureCode);
      
                // Save language preference
                await _secureStorageHandler.SetValue(StorageKeys.LanguagePreferenceKey, selectedLanguage.Id);

                // Update resource manager culture
                Localizer.SetCulture(culture);
            }
            catch (Exception ex)
            {
                var error = 100;
            }
            finally
            {
                IsBusy = false;

            }
        }
    }
}

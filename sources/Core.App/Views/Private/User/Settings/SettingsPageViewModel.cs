using CommunityToolkit.Mvvm.ComponentModel;
using Core.App.ViewModels;
using Services.Shared;
using Services.Shared.Interfaces;
using Services.Shared.UiModels;
using System.Collections.ObjectModel;


namespace Core.App.Views.Private.User.Settings
{
    public partial class SettingsPageViewModel : PrivateViewModelBase
    {
        [ObservableProperty]
        private DropdownItem _selectedLanguageItem = new DropdownItem();

        [ObservableProperty]
        private ObservableCollection<DropdownItem> _languageDropdownItems = new ObservableCollection<DropdownItem>
        {
            new DropdownItem
            {
                Id = 0,
                Label = Resx.Core.LabelEnglish
            },
            new DropdownItem
            {
                Id = 1,
                Label = Resx.Core.LabelGerman
            }
        };

        public SettingsPageViewModel(
            ICurrentUserService currentUserService, 
            ISecureStorageHandler secureStorageHandler,
            ILocalizationResourceManager localizationResourceManager
            )
            : base(currentUserService, secureStorageHandler, localizationResourceManager)
        {
            _ = LoadLanguagePreference();
        }

        partial void OnSelectedLanguageItemChanged(DropdownItem value)
        {
            if (value != null)
            {
                _ = ToggleLanguage(value);
            }

           
        }

        private async Task LoadLanguagePreference()
        {
            try
            {
                var savedLanguageId = await SecureStorageHandler.GetValue<int>(StorageKeys.LanguagePreferenceKey);

                var savedLanguage = LanguageDropdownItems.FirstOrDefault(x => x.Id == savedLanguageId);
                
                if (savedLanguage != null)
                {
                    SelectedLanguageItem = savedLanguage;
                }
                else
                {
                    SelectedLanguageItem = LanguageDropdownItems.First();
                }
            }
            catch
            {
                SelectedLanguageItem = LanguageDropdownItems.First();
            }
        }

    }
}

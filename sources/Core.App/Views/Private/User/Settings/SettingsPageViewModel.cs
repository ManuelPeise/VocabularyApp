using CommunityToolkit.Mvvm.ComponentModel;
using Core.App.ViewModels;
using Services.Shared;
using Services.Shared.Interfaces;
using Services.Shared.UiModels;
using Shared.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;


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

        [ObservableProperty]
        private UserSettingsModel _userSettings = new UserSettingsModel
        {
            Culture = CultureEnum.English,
            UseLocalDataStore = false,
            IsAutoDataSyncEnabled = false
        };

        public SettingsPageViewModel(
            ICurrentUserService currentUserService,
            ISecureStorageHandler secureStorageHandler,
            ILocalizationResourceManager localizationResourceManager
            )
            : base(currentUserService, secureStorageHandler, localizationResourceManager)
        {
            _ = Initialize();
        }

        partial void OnSelectedLanguageItemChanged(DropdownItem value)
        {
            if (value != null)
            {
                UserSettings.Culture = (CultureEnum)Enum.Parse(typeof(CultureEnum), value.Id.ToString());
            }
        }

        partial void OnUserSettingsChanged(UserSettingsModel? oldValue, UserSettingsModel newValue)
        {
            if (oldValue != null)
            {
                oldValue.PropertyChanged -= OnUserSettingsPropertyChanged;
            }

            if (newValue != null)
            {
                newValue.PropertyChanged += OnUserSettingsPropertyChanged;
            }
        }

        private async void OnUserSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(UserSettings.IsAutoDataSyncEnabled))
                {
                    IsBusy = true;

                    await CurrentUserService.UpdateUserSettings(UserSettings);

                    IsBusy = false;
                }
                else if (e.PropertyName == nameof(UserSettings.UseLocalDataStore))
                {
                    IsBusy = true;

                    await CurrentUserService.UpdateUserSettings(UserSettings);

                    IsBusy = false;
                }
                else if (e.PropertyName == nameof(UserSettings.Culture))
                {
                    var selectedLanguage = LanguageDropdownItems.FirstOrDefault(x => x.Id == (int)UserSettings.Culture);
                    
                    if (selectedLanguage != null)
                    {
                        IsBusy = true;

                        await ToggleLanguage(selectedLanguage);
                        await CurrentUserService.UpdateUserSettings(UserSettings);

                        IsBusy = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (log, show error message, etc.)
                System.Diagnostics.Debug.WriteLine($"Error updating user settings: {ex.Message}");
            }
        }


        private async Task Initialize()
        {
            try
            {
                UserSettings = await CurrentUserService.GetCurrentUserSettings(CurrentUserService.UserData.UserId);

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

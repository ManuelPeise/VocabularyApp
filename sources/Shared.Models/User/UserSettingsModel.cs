using CommunityToolkit.Mvvm.ComponentModel;
using Shared.Enums;

namespace Shared.Models.User
{
    public partial class UserSettingsModel : ObservableObject
    {
        [ObservableProperty] private int _userId;
        [ObservableProperty] private CultureEnum _culture = CultureEnum.English;
        [ObservableProperty] private bool _useLocalDataStore = false;
        [ObservableProperty] private bool _isAutoDataSyncEnabled = false;
    }
}

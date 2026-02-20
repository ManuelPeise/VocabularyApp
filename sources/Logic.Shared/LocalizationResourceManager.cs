using Logic.Shared.Interfaces;
using System.ComponentModel;
using System.Globalization;

namespace Logic.Shared
{
    public class LocalizationResourceManager : ILocalizationResourceManager, INotifyPropertyChanged
    {
        private static LocalizationResourceManager? _instance;
        public static LocalizationResourceManager Instance => _instance ??= new LocalizationResourceManager();

        public event PropertyChangedEventHandler? PropertyChanged;

        public string this[string key] => Resx.Core.ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;

        public void SetCulture(CultureInfo culture)
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            Resx.Core.Culture = culture;

            // Benachrichtigt alle gebundenen UI-Elemente über die Änderung
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}

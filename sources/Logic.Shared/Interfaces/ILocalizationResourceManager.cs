using System.Globalization;

namespace Logic.Shared.Interfaces
{
    public interface ILocalizationResourceManager
    {
        string this[string key] { get; }
        void SetCulture(CultureInfo culture);
    }
}

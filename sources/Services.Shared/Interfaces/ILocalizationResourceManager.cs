using System.Globalization;

namespace Services.Shared.Interfaces
{
    public interface ILocalizationResourceManager
    {
        string this[string key] { get; }
        void SetCulture(CultureInfo culture);
    }
}

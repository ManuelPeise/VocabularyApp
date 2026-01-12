using System.Globalization;

namespace Core.App.Converters
{
    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is byte[] bytes && bytes.Length > 0)
            {
                var imageSource = ImageSource.FromStream(() => new MemoryStream(bytes));

                return imageSource;
            }

            return ImageSource.FromStream(() => new MemoryStream(Resx.Images.NoImage));
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

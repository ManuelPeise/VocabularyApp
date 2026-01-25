using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Core.App.Converters
{
    internal class StringToIconConverter : IValueConverter
    {
        internal static string UserIcon = "\uf007";
        internal static string EmailIcon = "\uf0e0";
        internal static string LockIcon = "\uf023";
        internal static string CalendarIcon = "\uf133";
        internal static string RefreshIcon = "\uf021";
        internal static string PhoneIcon = "\uf095";
        internal static string HomeIcon = "\uf015";

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null && value is string iconString)
            {
                switch (iconString)
                {
                    case "userIcon":
                        return UserIcon;
                    case "emailIcon":
                        return EmailIcon;
                    case "lockIcon":
                        return LockIcon;
                    case "calendarIcon":
                        return CalendarIcon;
                    case "refreshIcon":
                        return RefreshIcon;
                    case "phoneIcon":
                        return PhoneIcon;
                    case "homeIcon":
                        return HomeIcon;
                    default: return string.Empty;
                }
            }

            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace GigaIRC.Client.WPF.Util
{
    public class PortConverter : IValueConverter
    {
        public static PortConverter Instance { get; } = new PortConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToString(value, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value as string))
                return null;
            return System.Convert.ToUInt16(value, culture);
        }
    }
}

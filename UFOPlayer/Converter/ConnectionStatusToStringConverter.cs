using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.Converter
{
    public class ConnectionStatusToStringConverter : IValueConverter

    {
        object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Enum.Parse(typeof(ConnectionStatus), (string)value, true);
        }
    }
}

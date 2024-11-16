using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFOPlayer.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan.ToString(@"hh\:mm\:ss"); // Format as HH:MM:SS
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && TimeSpan.TryParse(str, out var timeSpan))
            {
                return timeSpan;
            }
            return TimeSpan.Zero;
        }
    }
}
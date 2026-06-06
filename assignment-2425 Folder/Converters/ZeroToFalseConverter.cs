using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace assignment_2425.Converters
{
    public class ZeroToFalseConverter : IValueConverter
    {
        // If the value (assumed to be an integer) is zero, return false; otherwise, return true.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue != 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace assignment_2425.Converters
{

    public class ChatBubbleColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sender = value as string;
            return sender == "You" ? Colors.LightBlue : Colors.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CybersecurityChatbotWPF
{
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isCompleted = value is bool && (bool)value;
            return isCompleted ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.Orange);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
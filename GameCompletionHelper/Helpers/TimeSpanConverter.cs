using GameCompletionHelper.Formatters;
using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace GameCompletionHelper.Helpers
{
    class FormatTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpan = value as TimeSpan?;
            if (!timeSpan.HasValue)
            {
                return "Wrong data type";
            }

            return TimeSpanFormatter.ConvertToString(timeSpan.Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
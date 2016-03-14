using System;
using System.Globalization;
using System.Windows.Data;
using GameCompletionHelper.Formatters;

namespace GameCompletionHelper.Helpers.Converters
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
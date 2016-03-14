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
            var span = timeSpan.Value;
            StringBuilder builder = new StringBuilder();

            if (span.Days > 0)
                builder.AppendFormat("{0} day{1} ", span.Days, GetS(span.Days));
            builder.AppendFormat("{0} hour{1} {2} minute{3} {4} second{5}",
                span.Hours, GetS(span.Hours),
                span.Minutes, GetS(span.Minutes),
                span.Seconds, GetS(span.Seconds)
                );

            return builder.ToString();
        }

        private string GetS(int value)
        {
            return value != 1 ? "s" : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
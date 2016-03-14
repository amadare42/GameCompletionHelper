using System;
using System.Text;

namespace GameCompletionHelper.Formatters
{
    static class DateTimeFormatter
    {
        public static string ConvertToString(DateTime time, bool includeAgo = false)
        {
            if (time == default(DateTime))
                return "Never";

            StringBuilder builder = new StringBuilder($"{time.ToString()}");
            if (includeAgo)
            {
                var ago = TimeSpanFormatter.ConvertToString(DateTime.Now - time, TimeSpanFormatter.FormatOptions.OmmitSeconds);
                builder.Append($" ({ago} ago)");
            }
            return builder.ToString();
        }

        [Flags]
        public enum DateTimeFormatterOptions
        {
            None,
            IncludeAgo
        }
    }
}
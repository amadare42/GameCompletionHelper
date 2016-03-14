using System;
using System.Text;

namespace GameCompletionHelper.Formatters
{
    static class TimeSpanFormatter
    {
        public static string ConvertToString(TimeSpan span, FormatOptions options = FormatOptions.None)
        {
            StringBuilder builder = new StringBuilder();
            var space = "";

            if (span.Days > 0)
            {
                builder.AppendFormat("{0} day{1}", span.Days, GetS(span.Days));
                space = " ";
            }

            if (span.Hours > 0)
            {
                builder.Append($"{space}{span.Hours} hour{GetS(span.Hours)}");
                space = " ";
            }

            if (span.Minutes > 0)
            {
                builder.AppendFormat($"{space}{span.Minutes} minute{GetS(span.Minutes)}");
                space = " ";
            }

            if (span.Seconds > 0 && !options.HasFlag(FormatOptions.OmmitSeconds))
                builder.AppendFormat($"{space}{span.Seconds} second{GetS(span.Seconds)}");

            return builder.ToString();
        }

        private static string GetS(int value)
        {
            return value != 1 ? "s" : string.Empty;
        }

        [Flags]
        public enum FormatOptions
        {
            None,
            OmmitSeconds
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCompletionHelper.Formatters
{
    static class TimeSpanFormatter
    {
        private const string DefaultFormat = "{0} hour{1} {2} minute{3} {4} second{5}";
        private const string OmmitedSecondsFormat = "{0} hour{1} {2} minute{3}";

        public static string ConvertToString(TimeSpan span, FormatOptions options = FormatOptions.None)
        {
            StringBuilder builder = new StringBuilder();

            if (span.Days > 0)
                builder.AppendFormat("{0} day{1} ", span.Days, GetS(span.Days));
            builder.AppendFormat(
                options.HasFlag(FormatOptions.OmmitSeconds)
                    ? OmmitedSecondsFormat
                    : DefaultFormat,
                span.Hours, GetS(span.Hours),
                span.Minutes, GetS(span.Minutes),
                span.Seconds, GetS(span.Seconds)
                );

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
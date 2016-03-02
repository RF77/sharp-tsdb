using System;

namespace Timeenator.Extensions
{
    public static class DateTimeExtensions
    {
        public static uint ToSecondsAfter1970Utc(this DateTime date)
        {
            return (uint)(date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
        public static long ToMiliSecondsAfter1970(this DateTime date)
        {
            return (long)(date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
        public static long ToMiliSecondsAfter1970Utc(this DateTime date)
        {
            return (long)(date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public static long? ToFileTimeUtc(this DateTime? date)
        {
            if (date == null) return null;
            return date.Value.ToFileTimeUtc();
        }
    }
}
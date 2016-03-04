using System;

namespace Timeenator.Extensions
{
    public static class LongExtensions
    {
        public static DateTime FromSecondsAfter1970ToDateTimeUtc(this long seconds)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromSeconds(seconds));
        }
        public static DateTime FromMilisecondsAfter1970ToDateTimeUtc(this long seconds)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(seconds));
        }

        public static DateTime? FromFileTimeUtcToDateTimeUtc(this long? fileTime)
        {
            if (fileTime == null) return null;
            return DateTime.FromFileTimeUtc(fileTime.Value);
        }

    }
}
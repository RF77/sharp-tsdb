using System;

namespace Timeenator.Impl
{
    public static class IntExtensions
    {
        public static DateTime FromSecondsAfter1970ToDateTimeUtc(this UInt32 seconds)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromSeconds(seconds));
        }
    }
}
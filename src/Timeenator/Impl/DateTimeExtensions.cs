using System;

namespace Timeenator.Impl
{
    public static class DateTimeExtensions
    {
        public static UInt32 ToSecondsAfter1970Utc(this DateTime date)
        {
            return (UInt32)(date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
        public static long ToMiliSecondsAfter1970(this DateTime date)
        {
            return (long)(date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
        public static long ToMiliSecondsAfter1970Utc(this DateTime date)
        {
            return (long)(date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}
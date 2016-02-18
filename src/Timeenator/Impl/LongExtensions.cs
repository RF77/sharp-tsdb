using System;

namespace Timeenator.Impl
{
    public static class LongExtensions
    {
        public static DateTime FromSecondsAfter1970ToDateTime(this long seconds)
        {
            return (new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc) + TimeSpan.FromSeconds(seconds)).ToLocalTime();
        }     
    }
}
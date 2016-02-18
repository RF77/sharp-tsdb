using System;
using System.Collections.Generic;
using System.Linq;

namespace Timeenator.Impl.Grouping
{
    public static class StartEndTimeExtensions
    {
        public static IReadOnlyList<StartEndTime> CombineByTolerance(this IReadOnlyList<StartEndTime> groups,
            TimeSpan tolerance)
        {
            var newGroups = new List<StartEndTime>(groups.Count);
            var previous = groups.First();
            for (var i = 1; i < groups.Count; i++)
            {
                var current = groups[i];
                if ((current.Start - previous.End) <= tolerance)
                {
                    previous = new StartEndTime(previous.Start, current.End > previous.End ? current.End : previous.End);
                }
                else
                {
                    newGroups.Add(previous);
                    previous = current;
                }
            }
            newGroups.Add(previous);

            return newGroups;
        }

        public static IReadOnlyList<StartEndTime> CombineByTolerance(this IReadOnlyList<StartEndTime> groups,
            string tolerance)
        {
            return groups.CombineByTolerance(tolerance.ToTimeSpan());
        }
    }
}
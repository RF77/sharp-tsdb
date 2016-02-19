using System;
using System.Collections.Generic;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    public interface IGroupSelector<T>  where T:struct
    {
        IGroupByStartEndTimesConfiguratorOptional<T> ByTrigger(Func<ISingleDataRow<T>, bool> predicate);
        IGroupByStartEndTimesConfiguratorOptional<T> ByTimeRanges(IReadOnlyList<StartEndTime> groupTimes);
    }
}
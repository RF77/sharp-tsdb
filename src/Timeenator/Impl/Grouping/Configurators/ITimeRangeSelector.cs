using System;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    public interface ITimeRangeSelector<T> where T : struct
    {
        IGroupByStartEndTimesConfiguratorOptional<T> ByTrigger(Func<ISingleDataRow<T>, bool> predicate);
    }
}
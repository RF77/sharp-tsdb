using System.Collections.Generic;

namespace Timeenator.Impl.Grouping.Configurators
{
    public interface IGroupByStartEndTimesConfigurator<T> where T : struct
    {
        IGroupByStartEndTimesConfiguratorOptional<T> ByTimeRanges(IReadOnlyList<StartEndTime> groupTimes);
    }
}
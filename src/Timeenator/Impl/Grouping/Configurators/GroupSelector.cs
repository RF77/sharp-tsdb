using System;
using System.Collections.Generic;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    public class GroupSelector<T> : IGroupSelector<T>, ITimeRangeSelector<T> where T : struct
    {
        private readonly IQuerySerie<T> _serie;

        public GroupSelector(IQuerySerie<T> serie)
        {
            _serie = serie;
        }

        public IGroupByTriggerConfigurator<T> ByTrigger => new GroupByTriggerConfigurator<T>(_serie);
        public IGroupByStartEndTimesConfigurator<T> ByTimeRanges => new GroupByStartEndTimesConfigurator<T>(_serie);
        public IGroupByTimeConfigurator<T> ByTime => new GroupByTimeConfigurator<T>(_serie);
        IGroupByStartEndTimesConfiguratorOptional<T> IGroupSelector<T>.ByTrigger(Func<ISingleDataRow<T>, bool> predicate)
        {
            return ByTrigger.TriggerWhen(predicate);
        }

        IGroupByStartEndTimesConfiguratorOptional<T> IGroupSelector<T>.ByTimeRanges(IReadOnlyList<StartEndTime> groupTimes)
        {
            return ByTimeRanges.ByTimeRanges(groupTimes);
        }

        IGroupByStartEndTimesConfiguratorOptional<T> ITimeRangeSelector<T>.ByTrigger(Func<ISingleDataRow<T>, bool> predicate)
        {
            return ByTrigger.TriggerWhen(predicate);
        }
    }
}
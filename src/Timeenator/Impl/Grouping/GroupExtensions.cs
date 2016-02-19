using System;
using System.Collections.Generic;
using Timeenator.Impl.Grouping.Configurators;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping
{
    public static class GroupExtensions
    {
        public static INullableQuerySerie<T> Group<T>(this IQuerySerie<T> serie, Func<IGroupSelector<T>, IExecutableGroup<T>> groupConfigurator)
            where T : struct
        {
            return groupConfigurator(new GroupSelector<T>(serie)).ExecuteGrouping();
        }

        public static IReadOnlyList<StartEndTime> TimeRanges<T>(this IQuerySerie<T> serie, Func<IGroupSelector<T>, IGroupByStartEndTimesConfiguratorOptional<T>> groupConfigurator)
    where T : struct
        {
            var groupTimesCreator = groupConfigurator(new GroupSelector<T>(serie)) as IGroupTimesCreator;

            return groupTimesCreator?.CreateGroupTimes() ?? new List<StartEndTime>();
        }
    }
}

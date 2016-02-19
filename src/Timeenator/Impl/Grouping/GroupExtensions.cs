using System;
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
    }
}

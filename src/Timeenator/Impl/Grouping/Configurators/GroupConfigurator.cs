using System;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    internal abstract class GroupConfigurator<T> : IGroupAggregationConfigurator<T>, IExecutableGroup<T>
        where T : struct
    {
        public Func<IQuerySerie<T>, T?> AggregationFunc { get; set; }

        public IExecutableGroup<T> Aggregate(Func<IQuerySerie<T>, T?> aggregationFunc)
        {
            AggregationFunc = aggregationFunc;
            return this;
        }

        public abstract INullableQuerySerie<T> ExecuteGrouping();
    }
}
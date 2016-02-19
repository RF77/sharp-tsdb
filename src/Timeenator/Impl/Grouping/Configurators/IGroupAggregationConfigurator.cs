using System;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    public interface IGroupAggregationConfigurator<T> where T : struct
    {
        IExecutableGroup<T> Aggregate(Func<IQuerySerie<T>, T?> aggregationFunc);
    }
}
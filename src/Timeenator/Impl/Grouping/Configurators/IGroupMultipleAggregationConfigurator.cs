using System;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    public interface IGroupMultipleAggregationConfigurator<T> : IGroupAggregationConfigurator<T> where T : struct
    {
        IExecutableGroup<T> AggregateToNewSerie(string name, Func<IQuerySerie<T>, T?> aggregationFunc);
    }
}
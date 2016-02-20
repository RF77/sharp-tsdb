using System;
using System.Collections.Generic;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    public abstract class GroupConfigurator<T> : IGroupMultipleAggregationConfigurator<T>, IExecutableGroup<T>
        where T : struct
    {
        private Dictionary<string, Func<IQuerySerie<T>, T?>> _aggregationsForNewSeries;

        protected GroupConfigurator(IQuerySerie<T> serie)
        {
            Serie = serie;
        }

        protected Dictionary<string, Func<IQuerySerie<T>, T?>> AggregationsForNewSeries
            => _aggregationsForNewSeries ??
               (_aggregationsForNewSeries = new Dictionary<string,Func<IQuerySerie<T>,T?>>());

        protected IQuerySerie<T> Serie { get; set; }
        public Func<IQuerySerie<T>, T?> AggregationFunc { get; set; }
        public Func<IQuerySerie<T>, ISingleDataRow<T>> ItemSelector { get; set; }
        public abstract INullableQuerySerie<T> ExecuteGrouping();

        public IExecutableGroup<T> Aggregate(Func<IQuerySerie<T>, T?> aggregationFunc)
        {
            AggregationFunc = aggregationFunc;
            return this;
        }

        public IExecutableGroup<T> AggregateToNewSerie(string name, Func<IQuerySerie<T>, T?> aggregationFunc)
        {
            AggregationsForNewSeries[name] = aggregationFunc;
            return this;
        }

        public IExecutableGroup<T> SelectItem(Func<IQuerySerie<T>, ISingleDataRow<T>> itemSelector)
        {
            ItemSelector = itemSelector;
            return this;
        }
    }
}
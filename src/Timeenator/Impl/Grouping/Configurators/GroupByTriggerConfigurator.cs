using System;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    internal class GroupByTriggerConfigurator<T> : GroupByStartEndTimesConfigurator<T>, IGroupByTriggerConfigurator<T>
         where T : struct
    {
        private readonly IQuerySerie<T> _serie;

        public GroupByTriggerConfigurator(IQuerySerie<T> serie)
        {
            _serie = serie;
        }

        public Func<ISingleDataRow<T>, bool> PredicateFunc { get; set; }

        public IGroupByStartEndTimesConfigurator<T> TriggerWhen(Func<ISingleDataRow<T>, bool> predicate)
        {
            PredicateFunc = predicate;
            return this;
        }

        public override INullableQuerySerie<T> ExecuteGrouping()
        {
            throw new NotImplementedException();
        }
    }
}
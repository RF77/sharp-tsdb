using System;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    internal class GroupByTriggerConfigurator<T> : GroupByStartEndTimesConfigurator<T>, IGroupByTriggerConfigurator<T>
         where T : struct
    {
        private readonly IQuerySerie<T> _serie;

        public GroupByTriggerConfigurator(IQuerySerie<T> serie):base(serie)
        {
        }

        public Func<ISingleDataRow<T>, bool> PredicateFunc { get; set; }

        public IGroupByStartEndTimesConfiguratorOptional<T> TriggerWhen(Func<ISingleDataRow<T>, bool> predicate)
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
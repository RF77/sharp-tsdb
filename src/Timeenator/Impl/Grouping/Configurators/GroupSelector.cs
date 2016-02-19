using Timeenator.Impl.Grouping.Configurators;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping
{
    public class GroupSelector<T> : IGroupSelector<T> where T : struct
    {
        private readonly IQuerySerie<T> _serie;

        public GroupSelector(IQuerySerie<T> serie)
        {
            _serie = serie;
        }

        public IGroupByTriggerConfigurator<T> ByTrigger()
        {
            return new GroupByTriggerConfigurator<T>(_serie);
        }
    }
}
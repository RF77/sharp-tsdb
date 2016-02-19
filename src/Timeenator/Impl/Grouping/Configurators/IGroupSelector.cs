using Timeenator.Impl.Grouping.Configurators;

namespace Timeenator.Impl.Grouping
{
    public interface IGroupSelector<T>  where T:struct
    {
        IGroupByTriggerConfigurator<T> ByTrigger();
    }
}
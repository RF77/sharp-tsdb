using System;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    public interface IGroupByTriggerConfigurator<T> where T:struct
    {
        IGroupByStartEndTimesConfigurator<T> TriggerWhen(Func<ISingleDataRow<T>, bool> predicate);
    }
}
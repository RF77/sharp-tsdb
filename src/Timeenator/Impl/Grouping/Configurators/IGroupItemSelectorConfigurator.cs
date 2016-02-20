using System;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    public interface IGroupItemSelectorConfigurator<T> where T : struct
    {
        IExecutableGroup<T> SelectItem(Func<IQuerySerie<T>, ISingleDataRow<T>> itemSelector);
    }
}
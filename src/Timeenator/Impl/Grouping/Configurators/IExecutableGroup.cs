using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    public interface IExecutableGroup<T> where T : struct
    {
        INullableQuerySerie<T> ExecuteGrouping();
    }
}
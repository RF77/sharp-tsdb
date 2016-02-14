using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IObjectQueryTable
    {
        IObjectQuerySerieBase TryGetSerie(string name);
    }

    public interface IQueryTableBase<T> : IObjectQueryTable where T:struct 
    {

    }

    public interface IQueryTable<T> : IQueryTableBase<T> where T : struct
    {
        new IQuerySerie<T> TryGetSerie(string name);
        IEnumerable<IQuerySerie<T>> Series { get; }
        void AddSerie(IQuerySerie<T> serie);
    }

    public interface INullableQueryTable<T> : IQueryTableBase<T> where T : struct
    {
        new INullableQuerySerie<T> TryGetSerie(string name);
        void AddSerie(INullableQuerySerie<T> serie);
    }
}
using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface INullableQueryTable<T> : IQueryTableBase<T> where T : struct
    {
        new IEnumerable<INullableQuerySerie<T>> Series { get; } 
        new INullableQuerySerie<T> TryGetSerie(string name);
        void AddSerie(INullableQuerySerie<T> serie);
        INullableQueryTable<T> MergeTable(INullableQueryTable<T> otherTable);
    }
}
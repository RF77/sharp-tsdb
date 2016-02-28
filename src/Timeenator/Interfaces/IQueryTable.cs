using System;
using System.Collections.Generic;

namespace Timeenator.Interfaces
{
    public interface IQueryTable<T> : IQueryTableBase<T> where T : struct
    {
        new IQuerySerie<T> TryGetSerie(string name);
        new IEnumerable<IQuerySerie<T>> Series { get; }
        void AddSerie(IQuerySerie<T> serie);
        INullableQueryTable<T> Transform(Func<IQuerySerie<T>, INullableQuerySerie<T>> doFunc);
    }
}
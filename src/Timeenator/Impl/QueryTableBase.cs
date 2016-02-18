using System.Collections.Generic;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    public abstract class QueryTableBase<T> : IQueryTableBase<T> where T : struct
    {
        public abstract IObjectQuerySerieBase TryGetSerie(string name);

        public IEnumerable<IObjectQuerySerie> Series => GetSeries();

        protected abstract IEnumerable<IObjectQuerySerie> GetSeries();
    }
}
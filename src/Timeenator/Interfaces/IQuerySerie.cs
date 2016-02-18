using System.Collections.Generic;

namespace Timeenator.Interfaces
{
    public interface IQuerySerie<T> : IObjectQuerySerie, IQuerySerieBase<T> where T:struct 
    {
        new IReadOnlyList<ISingleDataRow<T>> Rows { get; }

        IEnumerable<T> Values { get; }
    }
}
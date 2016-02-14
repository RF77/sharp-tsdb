using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IQuerySerie<T> : IObjectQuerySerie, IQuerySerieBase<T> where T:struct 
    {
        new IReadOnlyList<ISingleDataRow<T>> Rows { get; } 
    }
}
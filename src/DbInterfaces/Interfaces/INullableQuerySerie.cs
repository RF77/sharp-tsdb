using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface INullableQuerySerie<T>: IObjectQuerySerie, IQuerySerieBase<T> where T : struct
    {
        new IReadOnlyList<ISingleDataRow<T?>> Rows { get; }
        INullableQuerySerie<T> Clone(string serieName);
    }
}
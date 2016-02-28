using System;
using System.Collections.Generic;

namespace Timeenator.Interfaces
{
    public interface INullableQuerySerie<T>: IObjectQuerySerie, IQuerySerieBase<T> where T : struct
    {
        new IReadOnlyList<ISingleDataRow<T?>> Rows { get; }
        INullableQuerySerie<T> Clone(string serieName);
        INullableQuerySerie<T> Zip(INullableQuerySerie<T> secondQuery, string resultQueryName, Func<T?, T?, T?> transformAction);
        INullableQuerySerie<T> Alias(string name);
        INullableQuerySerie<T> Transform(Func<T?, T?> transformFunc);
    }
}
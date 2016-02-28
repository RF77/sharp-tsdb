using System;
using System.Collections.Generic;

namespace Timeenator.Interfaces
{
    public interface IQuerySerie<T> : IObjectQuerySerie, IQuerySerieBase<T> where T:struct 
    {
        new IReadOnlyList<ISingleDataRow<T>> Rows { get; }

        IEnumerable<T> Values { get; }
        IQuerySerie<T> IncludeLastRow();
        IQuerySerie<T> Where(Func<ISingleDataRow<T>, bool> predicate);
        IQuerySerie<T> WhereValue(Func<T, bool> predicate);
        IQuerySerie<T> Alias(string name);
        INullableQuerySerie<T> Transform(Func<T, T?> transformFunc);

        /// <summary>
        /// Normalize a saw tooth like series due to overflows or resets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resetValue">let it null to take the first value after the overflow, otherwise set the value explicitly</param>
        /// <returns></returns>
        IQuerySerie<T> NormalizeOverflows(double? resetValue = null);

        INullableQuerySerie<T> ToNullable();
    }
}
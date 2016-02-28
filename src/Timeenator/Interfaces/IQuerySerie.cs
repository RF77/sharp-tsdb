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
        INullableQuerySerie<T> CalcNullableValue(Func<T, T?> calculationFunc, string newSerieName = null);
        IQuerySerie<T> CalcValue(Func<T, T> calculationFunc, string newSerieName = null);
        T? First();
        T? Last();
        T? Max();
        T? Min();

        /// <summary>
        /// Mean of all measurement points without looking to timestamps
        /// </summary>
        /// <returns></returns>
        T? Mean();

        /// <summary>
        /// Mean of all measurement points with taking the time into account
        /// </summary>
        /// <returns></returns>
        T? MeanByTime();

        T? Difference();
        T? Derivative(TimeSpan timeSpan);
        T? Derivative(string timeSpanExpression);
        T? Median();
        T? Sum();

        /// <summary>
        /// Calculates the time span where a specified condition (predicate) is true
        /// e.g. TimeWhere(v => v == 9.6f)?.TotalMinutes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">true, if added to time span</param>
        /// <returns>Time Span / use for example TotalMinutes to get a value of type T again</returns>
        TimeSpan? TimeWhere(Func<T, bool> predicate);
    }
}
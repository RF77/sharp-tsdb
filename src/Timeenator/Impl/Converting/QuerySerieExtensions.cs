using System;
using System.Collections.Generic;
using System.Linq;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Converting
{
    public static class QuerySerieExtensions
    {
        public static INullableQuerySerie<T> Zip<T>(this INullableQuerySerie<T> firstQuery, INullableQuerySerie<T> secondQuery, string resultQueryName, Func<T?,T?,T?> transformAction) where T : struct
        {
            if (firstQuery.Rows.Count != secondQuery.Rows.Count) throw new ArgumentOutOfRangeException(nameof(firstQuery), "Zip with different length of row not possible");
            var resultRows = new List<ISingleDataRow<T?>>(secondQuery.Rows.Count);

            var result = new NullableQuerySerie<T>(resultRows, firstQuery).Alias(resultQueryName);
            for (int i = 0; i < firstQuery.Rows.Count; i++)
            {
                if (firstQuery.Rows[i].TimeUtc != secondQuery.Rows[i].TimeUtc) throw new ArgumentOutOfRangeException(nameof(firstQuery), "Zip with not aligned times");

                resultRows.Add(new SingleDataRow<T?>(firstQuery.Rows[i].TimeUtc, transformAction(firstQuery.Rows[i].Value, secondQuery.Rows[i].Value)));
            }
            return result;
        }

        public static IQuerySerie<T> IncludeLastRow<T>(this IQuerySerie<T> serie) where T : struct
        {
            if (serie.Rows.Any())
            {
                serie.LastRow = serie.Rows.Last();
            }
            return serie;
        }

        public static IQuerySerie<T> Where<T>(this IQuerySerie<T> serie, Func<ISingleDataRow<T>, bool> predicate) where T : struct
        {
            if (serie.Rows.Any())
            {
                serie = new QuerySerie<T>(serie.Rows.Where(predicate).ToList(), serie);
            }
            return serie;
        }

        public static IQuerySerie<T> WhereValue<T>(this IQuerySerie<T> serie, Func<T, bool> predicate) where T : struct
        {
            if (serie.Rows.Any())
            {
                serie = new QuerySerie<T>(serie.Rows.Where(i => predicate(i.Value)).ToList(), serie);
            }
            return serie;
        }

       

        private static void SetAlias<T>(IQuerySerieBase<T> serie, string name) where T : struct
        {
            if (serie.GroupName != null)
            {
                serie.Key = name;
            }
            else
            {
                serie.Name = name;
            }
        }

        public static IQuerySerie<T> Alias<T>(this IQuerySerie<T> serie, string name) where T : struct
        {
            SetAlias(serie, name);
            return serie;
        }

        public static INullableQuerySerie<T> Alias<T>(this INullableQuerySerie<T> serie, string name) where T : struct
        {
            SetAlias(serie, name);
            return serie;
        }

        public static INullableQuerySerie<T> ToNullable<T>(this IQuerySerie<T> serie) where T : struct
        {
            return new NullableQuerySerie<T>(serie.Rows.Select(i => new SingleDataRow<T?>(i.TimeUtc, i.Value)).ToList(), serie);
        }

        public static INullableQuerySerie<T> Transform<T>(this INullableQuerySerie<T> serie,
            Func<T?, T?> transformFunc) where T : struct
        {
            var newRows = new List<ISingleDataRow<T?>>(serie.Rows.Count);
            newRows.AddRange(serie.Rows.Select(r => new SingleDataRow<T?>(r.TimeUtc, transformFunc(r.Value))));
            return new NullableQuerySerie<T>(newRows, serie);
        }

        public static INullableQuerySerie<T> Transform<T>(this IQuerySerie<T> serie,
            Func<T, T?> transformFunc) where T : struct
        {
            var newRows = new List<ISingleDataRow<T?>>(serie.Rows.Count);
            newRows.AddRange(serie.Rows.Select(r => new SingleDataRow<T?>(r.TimeUtc, transformFunc(r.Value))));
            return new NullableQuerySerie<T>(newRows, serie);
        }

        /// <summary>
        /// Normalize a saw tooth like series due to overflows or resets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serie"></param>
        /// <param name="resetValue">let it null to take the first value after the overflow, otherwise set the value explicitly</param>
        /// <returns></returns>
        public static IQuerySerie<T> NormalizeOverflows<T>(this IQuerySerie<T> serie, double? resetValue = null) where T : struct
        {
            if (serie.Rows.Any())
            {
                var newRows = new List<ISingleDataRow<T>>(serie.Rows.Count);
                double offset = 0;
                double previousValue = serie.Rows.First().Value.ToDouble();
                foreach (var row in serie.Rows)
                {
                    double rowValue = row.Value.ToDouble();
                    if (previousValue > rowValue)
                    {
                        if (resetValue != null)
                        {
                            offset += previousValue - (rowValue - resetValue.Value);
                        }
                        else
                        {
                            offset += previousValue;
                        }
                        
                    }
                    newRows.Add(new SingleDataRow<T>(row.TimeUtc, (rowValue + offset).ToType<T>()));
                    previousValue = rowValue;
                }
                return new QuerySerie<T>(newRows, serie);
            }
            return serie;
        }
    }
}

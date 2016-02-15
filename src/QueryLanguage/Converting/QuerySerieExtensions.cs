using System;
using System.Collections.Generic;
using System.Linq;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;

namespace QueryLanguage.Converting
{
    public static class QuerySerieExtensions
    {
        public static INullableQuerySerie<T> Zip<T>(this INullableQuerySerie<T> firstQuery, INullableQuerySerie<T> secondQuery, string resultQueryName, Func<T?,T?,T?> transformAction) where T : struct
        {
            if (firstQuery.Rows.Count != secondQuery.Rows.Count) throw new ArgumentOutOfRangeException(nameof(firstQuery), "Zip with different length of row not possible");
            var resultRows = new List<ISingleDataRow<T?>>(secondQuery.Rows.Count);

            var result = new NullableQuerySerie<T>(resultRows, firstQuery);
            result.Name = resultQueryName;
            for (int i = 0; i < firstQuery.Rows.Count; i++)
            {
                if (firstQuery.Rows[i].Key != secondQuery.Rows[i].Key) throw new ArgumentOutOfRangeException(nameof(firstQuery), "Zip with not aligned times");

                resultRows.Add(new SingleDataRow<T?>(firstQuery.Rows[i].Key, transformAction(firstQuery.Rows[i].Value, secondQuery.Rows[i].Value)));
            }
            return result;
        }

        public static IQuerySerie<T> TakeLastRow<T>(this IQuerySerie<T> serie) where T : struct
        {
            if (serie.Rows.Any())
            {
                serie.LastRow = serie.Rows.Last();
            }
            return serie;
        }
    }
}

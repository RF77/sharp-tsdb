using System;
using System.Collections.Generic;
using System.Linq;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace Timeenator.Extensions.Converting
{
    public static class QueryTableExtensions
    {
        public static INullableQueryTable<T> MergeTables<T>(this IEnumerable<INullableQueryTable<T>> sourceTables)
            where T : struct
        {
            var newTable = new NullableQueryTable<T>();
            foreach (var groupedTable in sourceTables)
            {
                foreach (var serie in groupedTable.Series)
                {
                    newTable.AddSerie(serie);
                }
            }
            return newTable;
        }

        public static IEnumerable<INullableQueryTable<T>> Transform<T>(this IEnumerable<INullableQueryTable<T>> tables,
            Func<INullableQueryTable<T>, INullableQueryTable<T>> transformFunc) where T : struct
        {
            return tables.Select(transformFunc);
        }

        public static IEnumerable<IQueryTable<T>> Transform<T>(this IEnumerable<IQueryTable<T>> tables,
    Func<IQueryTable<T>, IQueryTable<T>> transformFunc) where T : struct
        {
            return tables.Select(transformFunc);
        }
    }
}
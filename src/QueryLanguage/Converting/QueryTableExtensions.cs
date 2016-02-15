using System;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;

namespace QueryLanguage.Converting
{
    public static class QueryTableExtensions
    {
        public static INullableQueryTable<T> Do<T>(this IQueryTable<T> sourceTable, Func<IQuerySerie<T>, INullableQuerySerie<T>> doFunc) where T : struct
        {
            var table = new NullableQueryTable<T>();
            foreach (var serie in sourceTable.Series)
            {
                table.AddSerie(doFunc(serie));
            }

            return table;
        }

        public static INullableQueryTable<T> ToNewTable<T>(this INullableQueryTable<T> sourceTable, Action<INullableQueryTable<T>, INullableQueryTable<T>> transformAction) where T : struct
        {
            var table = new NullableQueryTable<T>();
            transformAction(sourceTable, table);
            return table;
        }
    }
}
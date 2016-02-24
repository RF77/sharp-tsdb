using System;
using System.Collections.Generic;
using System.Linq;
using Timeenator.Impl.Grouping;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Converting
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

        /// <summary>
        /// Creates a new table from a time aligned source table
        /// Due to the dynamic implementation and with big data performance hits are possible
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceTable">Source table with keyed series</param>
        /// <param name="newSerieKeyOrName">Name of zipped serie in the new table</param>
        /// <param name="zipFunc">
        /// Given is a dynamic source table where you can get the current iteration value
        /// e.g. you have to series with the key "A" and "B" in the source table.
        /// In this case a lambda expression like 't => t.A + t.B' would add them
        /// serie A has {1,2,3,4}
        /// serie B has {2,2,2,4}
        /// zipped serie has {3,4,5,8}
        /// </param>
        /// <returns>New table with the zipped result</returns>
        public static INullableQueryTable<T> ZipToNew<T>(this INullableQueryTable<T> sourceTable, string newSerieKeyOrName, Func<dynamic, T?> zipFunc) where T : struct
        {
            var table = new NullableQueryTable<T>();
            var dynamicTable = new DynamicTableValues(sourceTable);
            var firstSerie = sourceTable.Series.First();
            var count = firstSerie.Rows.Count;
            var newRows = new List<ISingleDataRow<T?>>(count);
            var newSerie = new NullableQuerySerie<T>(newRows, firstSerie) {Name = newSerieKeyOrName};
            table.AddSerie(newSerie);
            for (int i = 0; i < count; i++)
            {
                dynamicTable.Index = i;
                T? newValue = zipFunc(dynamicTable);
                newRows.Add(new SingleDataRow<T?>(firstSerie.Rows[i].Time, newValue));
            }
            
            return table;
        }

        public static IReadOnlyList<INullableQueryTable<T>> GroupSeries<T>(this INullableQueryTable<T> sourceTable)
            where T : struct
        {
            List<INullableQueryTable<T>> newTables = new List<INullableQueryTable<T>>();
            var groupedTables = sourceTable.Series.ToLookup(i => i.GroupName);
            foreach (var groupedTable in groupedTables)
            {
                var table = new NullableQueryTable<T>();
                newTables.Add(table);
                foreach (var serie in groupedTable)
                {
                    table.AddSerie(serie);
                }
            }
            return newTables;
        }

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

        /// <summary>
        /// Adds a new zipped serie to the source table from a time aligned source table
        /// Due to the dynamic implementation and with big data performance hits are possible
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceTable">Source table with keyed series</param>
        /// <param name="newSerieKeyOrName">Name of zipped serie in the new table</param>
        /// <param name="zipFunc">
        /// Given is a dynamic source table where you can get the current iteration value
        /// e.g. you have to series with the key "A" and "B" in the source table.
        /// In this case a lambda expression like 't => t.A + t.B' would add them
        /// serie A has {1,2,3,4}
        /// serie B has {2,2,2,4}
        /// zipped serie has {3,4,5,8}
        /// </param>
        /// <returns>Source table with added serie</returns>
        public static INullableQueryTable<T> ZipAndAdd<T>(this INullableQueryTable<T> sourceTable, string newSerieKeyOrName,
            Func<dynamic, T?> zipFunc) where T : struct
        {
            return sourceTable.MergeTable(sourceTable.ZipToNew(newSerieKeyOrName, zipFunc));
        }

        public static INullableQueryTable<T> ToNewTable<T>(this INullableQueryTable<T> sourceTable, Action<INullableQueryTable<T>, INullableQueryTable<T>> transformAction) where T : struct
        {
            var table = new NullableQueryTable<T>();
            transformAction(sourceTable, table);
            return table;
        }
    }
}
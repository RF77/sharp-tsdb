using System;
using System.Collections.Generic;

namespace Timeenator.Interfaces
{
    public interface INullableQueryTable<T> : IQueryTableBase<T> where T : struct
    {
        new IEnumerable<INullableQuerySerie<T>> Series { get; } 
        new INullableQuerySerie<T> TryGetSerie(string name);
        INullableQueryTable<T> AddSerie(INullableQuerySerie<T> serie);
        INullableQueryTable<T> RemoveSerie(string name);
        INullableQueryTable<T> MergeTable(INullableQueryTable<T> otherTable);

        /// <summary>
        ///     Creates a new table from a time aligned source table
        ///     Due to the dynamic implementation and with big data performance hits are possible
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newSerieKeyOrName">Name of zipped serie in the new table</param>
        /// <param name="zipFunc">
        ///     Given is a dynamic source table where you can get the current iteration value
        ///     e.g. you have to series with the key "A" and "B" in the source table.
        ///     In this case a lambda expression like 't => t.A + t.B' would add them
        ///     serie A has {1,2,3,4}
        ///     serie B has {2,2,2,4}
        ///     zipped serie has {3,4,5,8}
        /// </param>
        /// <returns>New table with the zipped result</returns>
        INullableQueryTable<T> ZipToNew(string newSerieKeyOrName, Func<dynamic, T?> zipFunc);

        IReadOnlyList<INullableQueryTable<T>> GroupSeries();

        /// <summary>
        ///     Adds a new zipped serie to the source table from a time aligned source table
        ///     Due to the dynamic implementation and with big data performance hits are possible
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newSerieKeyOrName">Name of zipped serie in the new table</param>
        /// <param name="zipFunc">
        ///     Given is a dynamic source table where you can get the current iteration value
        ///     e.g. you have to series with the key "A" and "B" in the source table.
        ///     In this case a lambda expression like 't => t.A + t.B' would add them
        ///     serie A has {1,2,3,4}
        ///     serie B has {2,2,2,4}
        ///     zipped serie has {3,4,5,8}
        /// </param>
        /// <returns>Source table with added serie</returns>
        INullableQueryTable<T> ZipAndAdd(string newSerieKeyOrName,Func<dynamic, T?> zipFunc);

        INullableQueryTable<T> ToNewTable(Action<INullableQueryTable<T>, INullableQueryTable<T>> transformAction);
    }
}
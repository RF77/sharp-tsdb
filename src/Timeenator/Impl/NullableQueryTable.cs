// /*******************************************************************************
//  * Copyright (c) 2016 by RF77 (https://github.com/RF77)
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the Eclipse Public License v1.0
//  * which accompanies this distribution, and is available at
//  * http://www.eclipse.org/legal/epl-v10.html
//  *
//  * Contributors:
//  *    RF77 - initial API and implementation and/or initial documentation
//  *******************************************************************************/ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using Timeenator.Extensions.Converting;
using Timeenator.Impl.Grouping;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    public class NullableQueryTable<T> : QueryTableBase<T>, INullableQueryTable<T> where T : struct
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public NullableQueryTable()
        {
        }

        public NullableQueryTable(IEnumerable<INullableQuerySerie<T>> series)
        {
            foreach (var serie in series)
            {
                AddSerie(serie);
            }
        }

        public override IObjectQuerySerieBase GetOrCreateSerie(string name)
        {
            var serie = TryGetSerie(name);
            if (serie == null)
            {
                var firstSerie = Series.Values.FirstOrDefault();
                if (firstSerie != null)
                {
                    var newSerie = new NullableQuerySerie<T>(new List<ISingleDataRow<T?>>(firstSerie.Rows.Select(i => new SingleDataRow<T?>(i.Key, null))), firstSerie.StartTime, firstSerie.EndTime).Alias(name);
                    AddSerie(newSerie);
                    serie = newSerie;
                }
            }
            return serie;
        }

        public new IDictionary<string, INullableQuerySerie<T>> Series { get; } =
            new Dictionary<string, INullableQuerySerie<T>>();

        public override IObjectQuerySerieBase TryGetSerie(string name)
        {
            return ((INullableQueryTable<T>) this).TryGetSerie(name);
        }

        IEnumerable<INullableQuerySerie<T>> INullableQueryTable<T>.Series => Series.Values;
        IEnumerable<IObjectQuerySerie> IObjectQueryTable.Series => Series.Values;

        public INullableQueryTable<T> AddSerie(INullableQuerySerie<T> serie)
        {
            Series[serie.FullName] = serie;
            return this;
        }

        public INullableQueryTable<T> RemoveSerie(string name)
        {
            Series.Remove(TryGetSerie(name).FullName);
            return this;
        }

        public INullableQueryTable<T> MergeTable(INullableQueryTable<T> otherTable)
        {
            foreach (var serie in otherTable.Series)
            {
                Series[serie.FullName] = serie;
            }
            return this;
        }

        INullableQuerySerie<T> INullableQueryTable<T>.TryGetSerie(string name)
        {
            foreach (var serie in Series.Values)
            {
                if (serie.Key == name) return serie;
                if (serie.Name == name) return serie;
            }

            return null;
        }

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
        public INullableQueryTable<T> ZipToNew(string newSerieKeyOrName, Func<dynamic, T?> zipFunc)
        {
            var table = new NullableQueryTable<T>();
            var sourceTables = GroupSeries();
            var resultTables = new List<INullableQueryTable<T>>();
            foreach (var sourceTable in sourceTables)
            {
                var dynamicTable = new DynamicTableValues(sourceTable);
                var firstSerie = sourceTable.Series.First();
                var count = firstSerie.Rows.Count;
                var newRows = new List<ISingleDataRow<T?>>(count);
                var newSerie = new NullableQuerySerie<T>(newRows, firstSerie).Alias(newSerieKeyOrName);
                table.AddSerie(newSerie);
                for (var i = 0; i < count; i++)
                {
                    dynamicTable.Index = i;
                    var newValue = zipFunc(dynamicTable);
                    newRows.Add(new SingleDataRow<T?>(firstSerie.Rows[i].TimeUtc, newValue));
                }

                resultTables.Add(table);
            }
            return resultTables.MergeTables();
        }

        public INullableQueryTable<T> Calc(Action<dynamic> zipFunc)
        {
            var dynamicTable = new DynamicTableValues(this);
            var firstSerie = Series.Values.First();
            var count = firstSerie.Rows.Count;
            //TODO: why are series not the same length? bug has to be fixed
            for (var i = 0; i < count - 1/*TODO: remove*/; i++)
            {
                try
                {
                    dynamicTable.Index = i;
                    zipFunc(dynamicTable);
                }
                catch (Exception ex)
                {
                    //Logger.Warn($"Exception in index {i}: {ex.Message}");
                }
            }
            return this;
        }

        public INullableQueryTable<T> RemoveDbSeries()
        {
            var querySeries = Series.Values.Where(i => i.IsDbSerie).ToArray();
            foreach (var serie in querySeries)
            {
                Series.Remove(serie.FullName);
            }
            return this;
        }

        public IReadOnlyList<INullableQueryTable<T>> GroupSeries()
        {
            var newTables = new List<INullableQueryTable<T>>();
            var groupedTables = Series.Values.Where(i => i.GroupName != null).ToLookup(i => i.GroupName);
            foreach (var groupedTable in groupedTables)
            {
                var table = new NullableQueryTable<T>();
                newTables.Add(table);
                foreach (var serie in groupedTable)
                {
                    table.AddSerie(serie);
                }
            }

            //Non grouped series to own table
            var nonGroupedSeries = Series.Values.Where(i => i.GroupName == null).ToList();
            if (nonGroupedSeries.Any())
            {
                var tableNonGrouped = new NullableQueryTable<T>();
                foreach (var serie in nonGroupedSeries)
                {
                    tableNonGrouped.AddSerie(serie);
                }

                newTables.Add(tableNonGrouped);
            }

            return newTables;
        }

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
        public INullableQueryTable<T> ZipAndAdd(string newSerieKeyOrName, Func<object, T?> zipFunc)
        {
            return MergeTable(ZipToNew(newSerieKeyOrName, zipFunc));
        }

        public INullableQueryTable<T> ToNewTable(Action<INullableQueryTable<T>, INullableQueryTable<T>> transformAction)
        {
            var table = new NullableQueryTable<T>();
            transformAction(this, table);
            return table;
        }

        protected override IEnumerable<IObjectQuerySerie> GetSeries() => Series.Values;
    }
}
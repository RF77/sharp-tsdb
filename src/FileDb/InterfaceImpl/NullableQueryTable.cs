using System;
using System.Collections.Generic;
using System.Linq;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class NullableQueryTable<T> : QueryTableBase<T>, INullableQueryTable<T> where T : struct
    {
        public new IDictionary<string, INullableQuerySerie<T>> Series { get; } = new Dictionary<string, INullableQuerySerie<T>>();
        protected override IEnumerable<IObjectQuerySerie> GetSeries() => Series.Values;

        public override IObjectQuerySerieBase TryGetSerie(string name)
        {
            return ((INullableQueryTable<T>)this).TryGetSerie(name);
        }

        IEnumerable<INullableQuerySerie<T>> INullableQueryTable<T>.Series => Series.Values;

        IEnumerable<IObjectQuerySerie> IObjectQueryTable.Series => Series.Values;

        public INullableQueryTable<T> AddSerie(INullableQuerySerie<T> serie)
        {
            Series[serie.Name] = serie;
            return this;
        }

        public INullableQueryTable<T> RemoveSerie(string name)
        {
            Series.Remove(name);
            return this;
        }

        public INullableQueryTable<T> MergeTable(INullableQueryTable<T> otherTable)
        {
            foreach (var serie in otherTable.Series)
            {
                Series[serie.Name] = serie;
            }
            return this;
        }

        INullableQuerySerie<T> INullableQueryTable<T>.TryGetSerie(string name)
        {
            INullableQuerySerie<T> serie;
            if (Series.TryGetValue(name, out serie))
            {
                return serie;
            }
            return null;
        }
    }
}
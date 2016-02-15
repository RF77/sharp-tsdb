using System;
using System.Collections.Generic;
using System.Linq;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class NullableQueryTable<T> : QueryTableBase<T>, INullableQueryTable<T> where T : struct
    {
        public new IDictionary<string, INullableQuerySerie<T>> Series { get; } = new Dictionary<string, INullableQuerySerie<T>>();
        protected override IEnumerable<IObjectQuerySerie> GetSeries() => Series.OfType<IObjectQuerySerie>();

        public override IObjectQuerySerieBase TryGetSerie(string name)
        {
            return ((INullableQueryTable<T>)this).TryGetSerie(name);
        }

        IEnumerable<IObjectQuerySerie> IObjectQueryTable.Series => Series.OfType<IObjectQuerySerie>();

        public void AddSerie(INullableQuerySerie<T> serie)
        {
            Series[serie.Name] = serie;
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
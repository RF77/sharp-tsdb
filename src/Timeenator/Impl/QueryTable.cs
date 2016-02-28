using System;
using System.Collections.Generic;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    public class QueryTable<T> : QueryTableBase<T>, IQueryTable<T> where T : struct
    {
        public new IDictionary<string, IQuerySerie<T>> Series { get; } = new Dictionary<string, IQuerySerie<T>>();
        protected override IEnumerable<IObjectQuerySerie> GetSeries() => Series.Values;


        public void AddSerie(IQuerySerie<T> serie)
        {
            Series[serie.Name] = serie;
        }

        IQuerySerie<T> IQueryTable<T>.TryGetSerie(string name)
        {
            IQuerySerie<T> serie;
            if (Series.TryGetValue(name, out serie))
            {
                return serie;
            }
            return null;
        }

        IEnumerable<IObjectQuerySerie> IObjectQueryTable.Series => Series.Values;

        IEnumerable<IQuerySerie<T>> IQueryTable<T>.Series => Series.Values;

        public override IObjectQuerySerieBase TryGetSerie(string name)
        {
            return ((IQueryTable<T>)this).TryGetSerie(name);
        }

        public INullableQueryTable<T> Transform(Func<IQuerySerie<T>, INullableQuerySerie<T>> doFunc)
        {
            var table = new NullableQueryTable<T>();
            foreach (var serie in Series.Values)
            {
                table.AddSerie(doFunc(serie));
            }

            return table;
        }
    }
}
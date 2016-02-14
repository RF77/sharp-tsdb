using System.Collections.Generic;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class NullableQueryTable<T> : QueryTableBase<T>, INullableQueryTable<T> where T : struct
    {
        public IDictionary<string, INullableQuerySerie<T>> Series { get; } = new Dictionary<string, INullableQuerySerie<T>>();
        public override IObjectQuerySerieBase TryGetSerie(string name)
        {
            return ((INullableQueryTable<T>)this).TryGetSerie(name);
        }

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
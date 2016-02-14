using System.Collections.Generic;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class QueryTable<T> : QueryTableBase<T>, IQueryTable<T> where T : struct
    {
        public IDictionary<string, IQuerySerie<T>> Series { get; } = new Dictionary<string, IQuerySerie<T>>();
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

        IEnumerable<IQuerySerie<T>> IQueryTable<T>.Series => Series.Values;

        public override IObjectQuerySerieBase TryGetSerie(string name)
        {
            return ((IQueryTable<T>)this).TryGetSerie(name);
        }
    }
}
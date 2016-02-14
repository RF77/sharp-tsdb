using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public abstract class QueryTableBase<T> : IQueryTableBase<T> where T : struct
    {
        public abstract IObjectQuerySerieBase TryGetSerie(string name);
    }
}
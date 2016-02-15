namespace DbInterfaces.Interfaces
{
    public interface INullableQueryTable<T> : IQueryTableBase<T> where T : struct
    {
        new INullableQuerySerie<T> TryGetSerie(string name);
        void AddSerie(INullableQuerySerie<T> serie);
    }
}
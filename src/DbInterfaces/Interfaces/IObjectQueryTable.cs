using System.Collections;
using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IObjectQueryTable : IQueryResult
    {
        IObjectQuerySerieBase TryGetSerie(string name);
        IEnumerable<IObjectQuerySerie> Series { get; }
    }
}
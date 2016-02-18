using System.Collections.Generic;

namespace Timeenator.Interfaces
{
    public interface IObjectQueryTable : IQueryResult
    {
        IObjectQuerySerieBase TryGetSerie(string name);
        IEnumerable<IObjectQuerySerie> Series { get; }
    }
}
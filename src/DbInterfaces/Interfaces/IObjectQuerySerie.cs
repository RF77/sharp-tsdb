using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IObjectQuerySerie : IObjectQuerySerieBase
    {
        IReadOnlyList<IObjectSingleDataRow> Rows { get; } 
    }
}
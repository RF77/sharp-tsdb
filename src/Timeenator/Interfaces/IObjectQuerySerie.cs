using System.Collections.Generic;

namespace Timeenator.Interfaces
{
    public interface IObjectQuerySerie : IObjectQuerySerieBase
    {
        IReadOnlyList<IObjectSingleDataRow> Rows { get; }

    }
}
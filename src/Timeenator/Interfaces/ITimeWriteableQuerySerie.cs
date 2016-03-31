using System;

namespace Timeenator.Interfaces
{
    public interface ITimeWriteableQuerySerie : IObjectQuerySerieBase
    {
        new DateTime? StartTime { get; set; }
        new DateTime? EndTime { get; set; }

    }
}
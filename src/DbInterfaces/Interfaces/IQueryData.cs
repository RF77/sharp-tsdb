using System;
using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IQueryData<T> where T:struct 
    {
        IReadOnlyList<ISingleDataRow<T>> Rows { get; } 
        DateTime? StartTime { get; }
        DateTime? StopTime { get; }
    }
}
using System;
using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IQueryData<T> : IQueryDataBase<T> where T:struct 
    {
        IReadOnlyList<ISingleDataRow<T>> Rows { get; } 
    }

    public interface INullableQueryData<T>:IQueryDataBase<T> where T : struct
    {
        IReadOnlyList<ISingleDataRow<T?>> Rows { get; }
    }

    public interface IQueryDataBase<T> where T : struct
    {
        DateTime? StartTime { get; }
        DateTime? StopTime { get; }

        /// <summary>
        /// Last value before the start time or null
        /// </summary>
        ISingleDataRow<T> PreviousRow { get; set; }

        /// <summary>
        /// first value after end time or null
        /// </summary>
        ISingleDataRow<T> NextRow { get; set; }

        string Name { get; set; }
    }
   
}
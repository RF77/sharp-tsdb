using System;
using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IObjectQueryData : IObjectQueryDataBase
    {
        IReadOnlyList<IObjectSingleDataRow> Rows { get; } 
    }

    public interface IQueryData<T> : IObjectQueryData, IQueryDataBase<T> where T:struct 
    {
        new IReadOnlyList<ISingleDataRow<T>> Rows { get; } 
    }

    public interface INullableQueryData<T>: IObjectQueryData, IQueryDataBase<T> where T : struct
    {
        new IReadOnlyList<ISingleDataRow<T?>> Rows { get; }
    }

    public interface IQueryDataBase<T> : IObjectQueryDataBase where T : struct
    {

        /// <summary>
        /// Last value before the start time or null
        /// </summary>
        new ISingleDataRow<T> PreviousRow { get; set; }

        /// <summary>
        /// first value after end time or null
        /// </summary>
        new ISingleDataRow<T> NextRow { get; set; }

    }

    public interface IObjectQueryDataBase
    {
        DateTime? StartTime { get; }
        DateTime? StopTime { get; }
        string Name { get; set; }

        /// <summary>
        /// Last value before the start time or null
        /// </summary>
        IObjectSingleDataRow PreviousRow { get;  }

        /// <summary>
        /// first value after end time or null
        /// </summary>
        IObjectSingleDataRow NextRow { get;  }

    }
}
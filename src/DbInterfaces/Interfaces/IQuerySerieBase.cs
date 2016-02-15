using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IQuerySerieBase<T> : IObjectQuerySerieBase where T : struct
    {

        /// <summary>
        /// Last value before the start time or null
        /// </summary>
        new ISingleDataRow<T> PreviousRow { get; set; }

        /// <summary>
        /// first value after end time or null
        /// </summary>
        new ISingleDataRow<T> NextRow { get; set; }

        /// <summary>
        /// defaults to null, but can be explicitly set to show in a chart the current value
        /// </summary>
        new ISingleDataRow<T> LastRow { get; set; }
    }
}
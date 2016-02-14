using System;

namespace DbInterfaces.Interfaces
{
    public interface IObjectQuerySerieBase
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
using System;

namespace Timeenator.Interfaces
{
    public interface IObjectQuerySerieBase : IQueryResult
    {
        DateTime? StartTime { get; }
        DateTime? EndTime { get; }
        string Name { get; set; }
        string GroupName { get; set; }
        string Key { get; set; }
        string OriginalName { get; }
        string FullName { get; }

        /// <summary>
        /// Last value before the start time or null
        /// </summary>
        IObjectSingleDataRow PreviousRow { get;  }

        /// <summary>
        /// first value after end time or null
        /// </summary>
        IObjectSingleDataRow NextRow { get;  }

        /// <summary>
        /// defaults to null, but can be explicitly set to show in a chart the current value
        /// </summary>
        IObjectSingleDataRow LastRow { get; }

        object this[int index]
        {
            get; set;
        }

    }
}
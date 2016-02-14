using System;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class QuerySerieBase<T>:IQuerySerieBase<T> where T : struct
    {
        public DateTime? StartTime { get; }
        public DateTime? StopTime { get; }

        /// <summary>
        /// Last value before the start time or null
        /// </summary>
        public ISingleDataRow<T> PreviousRow { get; set; }

        IObjectSingleDataRow IObjectQuerySerieBase.NextRow => NextRow;

        IObjectSingleDataRow IObjectQuerySerieBase.PreviousRow => PreviousRow;

        /// <summary>
        /// first value after end time or null
        /// </summary>
        public ISingleDataRow<T> NextRow { get; set; }

        /// <summary>
        /// Name of measurement
        /// </summary>
        public string Name { get; set; }

        public QuerySerieBase(DateTime? startTime, DateTime? stopTime)
        {
            StartTime = startTime;
            StopTime = stopTime;
        }

        protected QuerySerieBase(IQuerySerieBase<T> serie)
        {
            StartTime = serie.StartTime;
            StopTime = serie.StopTime;
            Name = serie.Name;
            NextRow = serie.NextRow;
            PreviousRow = serie.PreviousRow;
        }
    }
}
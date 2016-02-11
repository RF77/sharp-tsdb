using System;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class QueryDataBase<T>:IQueryDataBase<T> where T : struct
    {
        public DateTime? StartTime { get; }
        public DateTime? StopTime { get; }

        /// <summary>
        /// Last value before the start time or null
        /// </summary>
        public ISingleDataRow<T> PreviousRow { get; set; }

        /// <summary>
        /// first value after end time or null
        /// </summary>
        public ISingleDataRow<T> NextRow { get; set; }

        /// <summary>
        /// Name of measurement
        /// </summary>
        public string Name { get; set; }

        public QueryDataBase(DateTime? startTime, DateTime? stopTime)
        {
            StartTime = startTime;
            StopTime = stopTime;
        }

        protected QueryDataBase(IQueryDataBase<T> data)
        {
            StartTime = data.StartTime;
            StopTime = data.StopTime;
            Name = data.Name;
            NextRow = data.NextRow;
            PreviousRow = data.PreviousRow;
        }
    }
}
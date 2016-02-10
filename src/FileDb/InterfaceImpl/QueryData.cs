using System;
using System.Collections.Generic;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class QueryData<T> : IQueryData<T> where T : struct
    {
        public IReadOnlyList<ISingleDataRow<T>> Rows { get; }
        public DateTime? StartTime { get; }
        public DateTime? StopTime { get; }

        public QueryData(IReadOnlyList<ISingleDataRow<T>> rows, DateTime? startTime, DateTime? stopTime)
        {
            Rows = rows;
            StartTime = startTime;
            StopTime = stopTime;
        }
    }
}
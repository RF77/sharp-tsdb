using System;
using System.Collections.Generic;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class QueryData<T> : QueryDataBase<T>, IQueryData<T> where T : struct
    {
        public IReadOnlyList<ISingleDataRow<T>> Rows { get; }
       
        public QueryData(IReadOnlyList<ISingleDataRow<T>> rows, DateTime? startTime, DateTime? stopTime)
            :base(startTime, stopTime)

        {
            Rows = rows;
        }
    }
}
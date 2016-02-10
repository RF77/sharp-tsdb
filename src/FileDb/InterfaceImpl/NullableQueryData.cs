using System;
using System.Collections.Generic;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class NullableQueryData<T> : QueryDataBase<T>, INullableQueryData<T> where T : struct
    {
        public IReadOnlyList<ISingleDataRow<T?>> Rows { get; }

        public NullableQueryData(IReadOnlyList<ISingleDataRow<T?>> rows, DateTime? startTime, DateTime? stopTime)
            : base(startTime, stopTime)

        {
            Rows = rows;
        }
    }
}
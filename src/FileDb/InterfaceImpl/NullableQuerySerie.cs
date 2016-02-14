using System;
using System.Collections.Generic;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class NullableQuerySerie<T> : QuerySerieBase<T>, INullableQuerySerie<T> where T : struct
    {
        public IReadOnlyList<ISingleDataRow<T?>> Rows { get; }

        public NullableQuerySerie(IReadOnlyList<ISingleDataRow<T?>> rows, DateTime? startTime, DateTime? stopTime)
            : base(startTime, stopTime)

        {
            Rows = rows;
        }

        public NullableQuerySerie(IReadOnlyList<ISingleDataRow<T?>> result, IQuerySerie<T> olddata) : base(olddata)
        {
            Rows = result;
        }

        public NullableQuerySerie(IReadOnlyList<ISingleDataRow<T?>> result, INullableQuerySerie<T> olddata) : base(olddata)
        {
            Rows = result;
        }

        IReadOnlyList<IObjectSingleDataRow> IObjectQuerySerie.Rows => Rows;
    }
}
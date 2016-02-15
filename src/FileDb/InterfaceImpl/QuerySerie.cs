using System;
using System.Collections.Generic;
using System.Linq;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class QuerySerie<T> : QuerySerieBase<T>, IQuerySerie<T> where T : struct
    {
        public IReadOnlyList<ISingleDataRow<T>> Rows { get; }
        public IEnumerable<T> Values => Rows.Select(i => i.Value); 

        public QuerySerie(IReadOnlyList<ISingleDataRow<T>> rows, DateTime? startTime, DateTime? stopTime)
            :base(startTime, stopTime)

        {
            Rows = rows;
        }

        public QuerySerie(IReadOnlyList<ISingleDataRow<T>> rows, IQuerySerieBase<T> oldSerie) : base(oldSerie)
        {
            Rows = rows;
        }

        IReadOnlyList<IObjectSingleDataRow> IObjectQuerySerie.Rows => Rows;

    }
}
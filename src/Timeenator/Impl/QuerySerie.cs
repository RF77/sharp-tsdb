using System;
using System.Collections.Generic;
using System.Linq;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    public class QuerySerie<T> : QuerySerieBase<T>, IQuerySerie<T> where T : struct
    {
        public IReadOnlyList<ISingleDataRow<T>> Rows { get; }

        public override object this[int index]
        {
            get { return Rows[index].Value; }
            set
            {
                Rows[index].Value = (T)Convert.ChangeType(value, typeof(T));
            }
        }

        public IEnumerable<T> Values => Rows.Select(i => i.Value); 

        public QuerySerie(IReadOnlyList<ISingleDataRow<T>> rows, DateTime? startTime, DateTime? endTime)
            :base(startTime, endTime)

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
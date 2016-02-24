using System;
using System.Collections.Generic;
using System.Diagnostics;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    [DebuggerDisplay("{FullName} ({Rows.Count()})")] 
    public class NullableQuerySerie<T> : QuerySerieBase<T>, INullableQuerySerie<T> where T : struct
    {
        public IReadOnlyList<ISingleDataRow<T?>> Rows { get; }

        public override object this[int index]
        {
            get { return Rows[index].Value; }
            set
            {
                if (value == null)
                {
                    Rows[index].Value = null;
                }
                else
                {
                    Rows[index].Value =  (T)Convert.ChangeType(value, typeof(T));
                }
            }
        }

        public INullableQuerySerie<T> Clone(string serieName)
        {
            var serie = new NullableQuerySerie<T>(Rows, this) {Name = serieName};
            return serie;
        }

        public NullableQuerySerie(IReadOnlyList<ISingleDataRow<T?>> rows, DateTime? startTime, DateTime? endTime)
            : base(startTime, endTime)

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
using System;
using System.Collections.Generic;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
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
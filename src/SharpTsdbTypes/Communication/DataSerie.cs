using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Timeenator.Extensions;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace SharpTsdbTypes.Communication
{
    [DataContract]
    public class DataSerie
    {
        public DataSerie()
        {
        }

        public DataSerie(IObjectQuerySerie querySerie)
        {
            Rows = new DataRows(querySerie.Rows);
            Name = querySerie.FullName;
            StartTime = querySerie.StartTime.ToFileTimeUtc();
            EndTime = querySerie.EndTime.ToFileTimeUtc();
            PreviousRow = querySerie.PreviousRow?.ToArray();
            NextRow = querySerie.NextRow?.ToArray();
        }

        public IQuerySerie<T> ToQuerySerie<T>() where T : struct
        {
            return new QuerySerie<T>(Rows.AsTyped<T>().ToList(), StartTime.FromFileTimeUtcToDateTimeUtc(), EndTime.FromFileTimeUtcToDateTimeUtc())
            {
                PreviousRow = PreviousRow.ToSingleDataRow<T>(),
                NextRow = NextRow.ToSingleDataRow<T>()
            };
        }

        public INullableQuerySerie<T> ToNullableQuerySerie<T>() where T : struct
        {
            return new NullableQuerySerie<T>(Rows.AsNullableTyped<T>().ToList(), StartTime.FromFileTimeUtcToDateTimeUtc(), EndTime.FromFileTimeUtcToDateTimeUtc())
            {
                PreviousRow = PreviousRow?.ToSingleDataRow<T>(),
                NextRow = NextRow?.ToSingleDataRow<T>(),
                Name = Name
            };
        }


        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public long? StartTime { get; set; }

        [DataMember]
        public long? EndTime { get; set; }

        /// <summary>
        /// Last value before the start time or null
        /// </summary>
        [DataMember]
        public object[] PreviousRow { get; set; }

        [DataMember]
        public object[] NextRow { get; set; }

        [DataMember]
        public DataRows Rows { get; set; }

    }
}
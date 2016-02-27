using System;
using System.Linq;
using System.Runtime.Serialization;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace SharpTsdbTypes.Communication
{
    [DataContract]
    public class DataSerie
    {
        public DataSerie(IObjectQuerySerie querySerie)
        {
            Rows = new DataRows(querySerie.Rows.OfType<IDataRow>());
            Name = querySerie.FullName;
            StartTime = querySerie.StartTime;
            EndTime = querySerie.EndTime;
            PreviousRow = querySerie.PreviousRow.ToArray();
            NextRow = querySerie.NextRow.ToArray();
        }

        public IQuerySerie<T> ToQuerySerie<T>() where T : struct
        {
            return new QuerySerie<T>(Rows.AsTyped<T>().ToList(), StartTime, EndTime)
            {
                PreviousRow = PreviousRow.ToSingleDataRow<T>(),
                NextRow = NextRow.ToSingleDataRow<T>()
            };
        }

        [DataMember]
        public DataRows Rows { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime? StartTime { get; set; }

        [DataMember]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Last value before the start time or null
        /// </summary>
        [DataMember]
        public object[] PreviousRow { get; set; }

        [DataMember]
        public object[] NextRow { get; set; }
    }
}
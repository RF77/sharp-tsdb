using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Timeenator.Extensions.Converting;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace SharpTsdbTypes.Communication
{
    [DataContract]
    public class DataRows
    {
        [DataMember]
        public List<object[]> Rows { get; set; }

        public DataRows()
        {
            
        }

        public DataRows(IEnumerable<IDataRow> rows)
        {
            Rows = rows.Select(i => new[] {i.Key.ToFileTimeUtc(), i.Value}).ToList();
        }

        public IEnumerable<ISingleDataRow<T>> AsTyped<T>() where T : struct
        {
            return Rows?.Select(i => new SingleDataRow<T>(ToDateTime(i), i[1].ToType<T>()));
        }

        private static DateTime ToDateTime(object[] i)
        {
            return DateTime.FromFileTimeUtc((long)i[0]);
        }

        public IEnumerable<ISingleDataRow<T?>> AsNullableTyped<T>() where T : struct
        {
            return Rows?.Select(i => new SingleDataRow<T?>(ToDateTime(i), i[1]?.ToType<T>()));
        }

        public IEnumerable<IDataRow> AsIDataRows()
        {
            return Rows?.Select(i => new DataRow { Key = ToDateTime(i), Value = i[1]});
        }
    }
}

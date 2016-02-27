using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Timeenator.Impl;
using Timeenator.Impl.Converting;
using Timeenator.Interfaces;

namespace SharpTsdbTypes.Communication
{
    [DataContract]
    public class DataRows
    {
        [DataMember]
        public List<object[]> Rows { get; }

        public DataRows(IEnumerable<IDataRow> rows)
        {
            Rows = rows.Select(i => new[] {i.Key, i.Value}).ToList();
        }

        public IEnumerable<ISingleDataRow<T>> AsTyped<T>() where T : struct
        {
            return Rows?.Select(i => new SingleDataRow<T>((DateTime)i[0], i[1].ToType<T>()));
        }

        public IEnumerable<IDataRow> AsIDataRows()
        {
            return Rows?.Select(i => new DataRow { Key = (DateTime)i[0], Value = i[1]});
        }
    }
}

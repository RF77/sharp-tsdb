using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Timeenator.Interfaces;

namespace SharpTsdbTypes.Communication
{
    [DataContract]
    public class DataRows
    {
        private List<object[]> Rows { get; }
        public DataRows(IEnumerable<IDataRow> rows)
        {
            Rows = rows.Select(i => new[] {i.Key, i.Value}).ToList();
        }
    }
}

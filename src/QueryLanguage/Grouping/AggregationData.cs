using System;
using System.Collections.Generic;
using System.Linq;
using DbInterfaces.Interfaces;

namespace QueryLanguage.Grouping
{
    public class AggregationData<T> where T:struct 
    {
        public IReadOnlyList<ISingleDataRow<T>> Rows { get; set; }

        public T[] ValueArray
        {
            get { return Rows.Select(i => i.Value).ToArray(); }
        }
        public IEnumerable<T> Values
        {
            get { return Rows.Select(i => i.Value); }
        }

        public ISingleDataRow<T> Previous { get; set; }
        public ISingleDataRow<T> Next { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
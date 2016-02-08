using System;
using System.Collections.Generic;
using DbInterfaces.Interfaces;

namespace QueryLanguage.Grouping
{
    public class AggregationData<T>
    {
        public IReadOnlyList<ISingleDataRow<T>> Values { get; set; }
        public ISingleDataRow<T> Previous { get; set; }
        public ISingleDataRow<T> Next { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
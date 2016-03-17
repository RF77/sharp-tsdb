using System;

namespace Mqtt2SharpTsdb.Items
{
    public class MeasurementItem
    {
        public object CurrentValue { get; set; }
        public DateTime? LastChange { get; set; }
    }
}

using System;

namespace SharpTsdbClient.Data
{
    public class WritePoint
    {
        public DateTime t { get; set; }
        public object v { get; set; }
    }
}
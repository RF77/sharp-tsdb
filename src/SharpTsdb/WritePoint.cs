using System;

namespace SharpTsdb
{
    public class WritePoint
    {
        public DateTime t { get; set; }
        public object v { get; set; }
    }
}
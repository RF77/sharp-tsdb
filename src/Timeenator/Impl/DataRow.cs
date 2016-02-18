using System;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    public class DataRow : IDataRow
    {
        public DateTime Key { get; set; }
        public object Value { get; set; }
    }
}
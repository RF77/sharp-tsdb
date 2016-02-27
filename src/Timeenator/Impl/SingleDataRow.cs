using System;
using System.Diagnostics;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    [DebuggerDisplay("{Key}: {Value}")]
    public class SingleDataRow<T> : DataRow, ISingleDataRow<T>
    {
        public DateTime Time
        {
            get
            {
                return Key;
            }
            set { Key = value; }
        }

        public new T Value
        {
            get { return (T) base.Value; }
            set { base.Value =  value; }
        }

        public object[] ToArray()
        {
            return new object[] {Key, Value};
        }

        public SingleDataRow(DateTime time, T value)
        {
            Key = time;
            Value = value;
        }
    }
}
using System;
using System.Diagnostics;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    [DebuggerDisplay("{Key}: {Value}")]
    public class SingleDataRow<T> : DataRow, ISingleDataRow<T>
    {
        public new T Value
        {
            get { return (T) base.Value; }
            set { base.Value =  value; }
        }

        public SingleDataRow(DateTime time, T value)
        {
            Key = time;
            Value = value;
        }
    }
}
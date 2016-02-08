using System;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class SingleDataRow<T> : DataRow, ISingleDataRow<T>
    {

        public T Value
        {
            get { return (T) Values[0]; }
            set { Values = new object[] { value }; }
        }

        public SingleDataRow(DateTime time, T value)
        {
            Key = time;
            Value = value;
        }
    }
}
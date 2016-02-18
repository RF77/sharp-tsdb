using System;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class DataRow : IDataRow
    {
        public DateTime Key { get; set; }
        public object Value { get; set; }
    }
}
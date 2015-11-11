using System;

namespace DbInterfaces.Interfaces
{
    public interface IDataRow
    {
        DateTime Key { get; set; }
        object[] Values { get; set; }
    }
}
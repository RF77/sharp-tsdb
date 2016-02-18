using System;

namespace DbInterfaces.Interfaces
{
    public interface IDataRow
    {
        DateTime Key { get; set; }
        object Value { get; set; }
    }
}
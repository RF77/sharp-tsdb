using System;

namespace DbInterfaces.Interfaces
{
    public interface ISingleDataRow<T>
    {
        DateTime Key { get; set; }
        T Value { get; set; }
    }
}
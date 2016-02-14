using System;

namespace DbInterfaces.Interfaces
{
    public interface ISingleDataRow<T> : IObjectSingleDataRow
    {
        new T Value { get; set; }
    }

    public interface IObjectSingleDataRow
    {
        DateTime Key { get; set; }
        object Value { get;  }

    }
}
using System;

namespace Timeenator.Interfaces
{
    public interface IDataRow
    {
        DateTime Key { get; set; }
        object Value { get; set; }
    }
}
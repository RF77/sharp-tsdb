using System;

namespace Timeenator.Interfaces
{
    public interface IObjectSingleDataRow : IDataRow
    {
        DateTime TimeUtc { get; set; }

        object[] ToArray();
    }
}
using System;

namespace Timeenator.Interfaces
{
    public interface IObjectSingleDataRow : IDataRow
    {
        DateTime Time { get; set; }

        object[] ToArray();
    }
}
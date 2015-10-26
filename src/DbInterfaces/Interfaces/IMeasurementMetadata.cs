using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IMeasurementMetadata
    {
        string Name { get; set; }
        HashSet<string> Tags { get; }
        IEnumerable<IColumn> Columns { get; }
        string Id { get; }
    }
}
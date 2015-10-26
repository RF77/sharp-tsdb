using System;
using System.Collections;
using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IMeasurement
    {
        void AddDataPoints(IEnumerable<IDataRow> row);
        IEnumerable<IDataRow> GetDataPoints(DateTime? from, DateTime? to);
        IMeasurementMetadata Metadata { get; }
        string BinaryFilePath { get; }
    }
}
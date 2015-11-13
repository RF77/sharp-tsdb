using System;
using System.Collections;
using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IMeasurement
    {
        void AppendDataPoints(IEnumerable<IDataRow> row);
        IEnumerable<IDataRow> GetDataPoints(DateTime? from = null, DateTime? to = null);
        IMeasurementMetadata Metadata { get; }
        string BinaryFilePath { get; }
    }
}
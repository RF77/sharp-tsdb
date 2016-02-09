using System;
using System.Collections;
using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IMeasurement
    {
        void AppendDataPoints(IEnumerable<IDataRow> row);
        IEnumerable<ISingleDataRow<T>> GetDataPoints<T>(DateTime? from = null, DateTime? to = null);
        void ClearDataPoints();
        IMeasurementMetadata Metadata { get; }
        string BinaryFilePath { get; }
    }
}
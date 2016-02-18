using System;
using System.Collections;
using System.Collections.Generic;
using Timeenator.Interfaces;

namespace DbInterfaces.Interfaces
{
    public interface IMeasurement
    {
        void AppendDataPoints(IEnumerable<IDataRow> row);
        IQuerySerie<T> GetDataPoints<T>(DateTime? from = null, DateTime? to = null) where T : struct;
        IQuerySerie<T> GetDataPoints<T>(string timeExpression) where T : struct;
        void ClearDataPoints();
        IMeasurementMetadata Metadata { get; }
        string BinaryFilePath { get; }
    }
}
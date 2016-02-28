using System;
using System.Collections.Generic;
using Timeenator.Interfaces;

namespace DbInterfaces.Interfaces
{
    public interface IDb
    {
        IDbMetadata Metadata { get; }
        string Name { get; }
        void SaveMetadata();
        string MeasurementDirectory { get; }
        void CreateMeasurement(IMeasurementMetadata metadata);
        IMeasurement CreateMeasurement(string name, Type valueType);
        IMeasurement GetMeasurement(string name);
        IQuerySerie<T> GetSerie<T>(string measurementName, string timeExpression) where T : struct;
        IQuerySerie<T> GetSerie<T>(string measurementName) where T : struct;
        IQueryTable<T> GetTable<T>(string measurementRegex, string timeExpression) where T : struct;
        IObjectQueryTable GetTable(string measurementRegex, string timeExpression);
        IReadOnlyList<string> GetMeasurementNames();
        void DeleteMeasurement(string name);
        void DeleteAllMeasurements();
        IMeasurement GetOrCreateMeasurement(string name, string type = "float");
    }
}
using System;
using System.Collections.Generic;

namespace DbInterfaces.Interfaces
{
    public interface IDbMetadata
    {
        string Name { get; set; }
        Guid Id { get; set; }
        string DbPath { get; set; }
        string DbMetadataPath { get; }
        IMeasurement GetMeasurement(string name);
        void SetMeasurement(string name, IMeasurement measurement);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    [DataContract]
    public class DbMetadata : IDbMetadata
    {
        public DbMetadata()
        {
            Measurements = new Dictionary<string, Measurement>();
        }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string DbPath { get; set; }

        public string DbMetadataPath
        {
            get { return GetMetadataPath(DbPath); }
        }

        public IMeasurement GetMeasurement(string name)
        {
            return Measurements[name];
        }

        public void SetMeasurement(string name, IMeasurement measurement)
        {
            Measurements[name] = (Measurement) measurement;
        }

        [DataMember]
        public Dictionary<string, Measurement> Measurements { get; private set; }


        public static string GetMetadataPath(string dbPath)
        {
            return Path.Combine(dbPath, "Metadata.json");
        }

    }
}
using System;
using System.IO;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{


    [Serializable]
    public class DbMetadata : IDbMetadata
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string DbPath { get; set; }

        public string DbMetadataPath
        {
            get { return GetMetadataPath(DbPath); }
        }

        public static string GetMetadataPath(string dbPath)
        {
            return Path.Combine(dbPath, "Metadata.json");
        }

    }
}
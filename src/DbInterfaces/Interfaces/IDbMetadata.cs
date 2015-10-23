using System;

namespace DbInterfaces.Interfaces
{
    public interface IDbMetadata
    {
        string Name { get; set; }
        Guid Id { get; set; }
        string DbPath { get; set; }
        string DbMetadataPath { get; }
    }
}
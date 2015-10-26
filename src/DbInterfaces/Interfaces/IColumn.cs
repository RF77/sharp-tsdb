using System;

namespace DbInterfaces.Interfaces
{
    public interface IColumn
    {
        string Name { get; }
        Type ValueType { get; }
        byte Size { get; }
    }
}
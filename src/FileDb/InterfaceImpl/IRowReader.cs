using System.IO;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public interface IRowReader
    {
        ISingleDataRow<T> ReadRow<T>(BinaryReader reader);
    }
}
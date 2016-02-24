using System.IO;
using DbInterfaces.Interfaces;
using Timeenator.Interfaces;

namespace FileDb.InterfaceImpl
{
    public interface IRowReader
    {
        ISingleDataRow<T> ReadRow<T>(BinaryReader reader);
        IDataRow ReadRow(BinaryReader reader);
    }
}
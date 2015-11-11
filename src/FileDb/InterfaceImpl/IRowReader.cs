using System.IO;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public interface IRowReader
    {
        IDataRow ReadRow(BinaryReader reader);
    }
}
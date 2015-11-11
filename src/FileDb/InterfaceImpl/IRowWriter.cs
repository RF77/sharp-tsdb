using System.IO;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public interface IRowWriter
    {
        void WriteRow(BinaryWriter writer, IDataRow row);
    }
}
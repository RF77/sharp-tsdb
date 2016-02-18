using System.IO;
using DbInterfaces.Interfaces;
using Timeenator.Interfaces;

namespace FileDb.InterfaceImpl
{
    public interface IRowWriter
    {
        void WriteRow(BinaryWriter writer, IDataRow row);
    }
}
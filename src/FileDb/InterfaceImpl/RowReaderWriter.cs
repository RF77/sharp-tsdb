using System.IO;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public abstract class RowReaderWriter : IRowWriter, IRowReader
    {
        public int RowLength { get; set; }
        public abstract void WriteRow(BinaryWriter writer, IDataRow row);
        public abstract ISingleDataRow<T> ReadRow<T>(BinaryReader reader);
    }
}
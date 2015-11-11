using System.IO;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public abstract class RowReaderWriter : IRowWriter, IRowReader
    {
        protected int RowLength { get; set; }
        public abstract void WriteRow(BinaryWriter writer, IDataRow row);
        public abstract IDataRow ReadRow(BinaryReader reader);
    }
}
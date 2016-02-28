using System;
using System.IO;
using DbInterfaces.Interfaces;
using Timeenator.Interfaces;

namespace FileDb.InterfaceImpl
{
    public abstract class RowReaderWriter : IRowWriter, IRowReader
    {
        public int RowLength { get; set; }

        public virtual void WriteRow(BinaryWriter writer, IDataRow row)
        {
            writer.Write(row.Key.Ticks); //Time is always the same
        }

        protected static DateTime ReadDate(BinaryReader reader)
        {
            return DateTime.FromBinary(reader.ReadInt64());
        }

        public abstract ISingleDataRow<T> ReadRow<T>(BinaryReader reader);
        public abstract IDataRow ReadRow(BinaryReader reader);
    }
}
using System;
using System.IO;
using DbInterfaces.Interfaces;

namespace FileDb.InterfaceImpl
{
    public class FloatRowReaderWriter : RowReaderWriter
    {
        public FloatRowReaderWriter()
        {
            RowLength = 12;
        }

        public override void WriteRow(BinaryWriter writer, IDataRow row)
        {
            writer.Write(row.Key.Ticks);
            writer.Write(Convert.ToSingle(row.Value));
        }

        public override ISingleDataRow<T> ReadRow<T>(BinaryReader reader)
        {
            var row = new SingleDataRow<T>(DateTime.FromBinary(reader.ReadInt64()), (T) Convert.ChangeType(reader.ReadSingle(), typeof(T)));

            return row;
        }
    }
}
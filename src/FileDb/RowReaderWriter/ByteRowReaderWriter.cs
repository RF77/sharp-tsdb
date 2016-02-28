using System;
using System.IO;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace FileDb.RowReaderWriter
{
    public class ByteRowReaderWriter : RowReaderWriter
    {
        public ByteRowReaderWriter()
        {
            RowLength = 9;
        }

        public override void WriteRow(BinaryWriter writer, IDataRow row)
        {
            base.WriteRow(writer, row);
            writer.Write(Convert.ToByte(row.Value));
        }

        public override ISingleDataRow<T> ReadRow<T>(BinaryReader reader)
        {
            var row = new SingleDataRow<T>(ReadDate(reader), (T)Convert.ChangeType(reader.ReadByte(), typeof(T)));
            return row;
        }

        public override IDataRow ReadRow(BinaryReader reader)
        {
            return new DataRow { Key = ReadDate(reader), Value = reader.ReadByte() };
        }
    }
}
using System;
using System.IO;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace FileDb.RowReaderWriter
{
    public class DateTimeRowReaderWriter : RowReaderWriter
    {
        public DateTimeRowReaderWriter()
        {
            RowLength = 16;
        }

        public override void WriteRow(BinaryWriter writer, IDataRow row)
        {
            base.WriteRow(writer, row);
            writer.Write(Convert.ToInt64(((DateTime)row.Value).Ticks));
        }

        public override ISingleDataRow<T> ReadRow<T>(BinaryReader reader)
        {
            var row = new SingleDataRow<T>(ReadDate(reader), (T)Convert.ChangeType(DateTime.FromBinary(reader.ReadInt64()), typeof(T)));
            return row;
        }

        public override IDataRow ReadRow(BinaryReader reader)
        {
            return new DataRow { Key = ReadDate(reader), Value = DateTime.FromBinary(reader.ReadInt64()) };
        }
    }
}
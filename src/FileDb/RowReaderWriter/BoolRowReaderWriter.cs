using System;
using System.IO;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace FileDb.RowReaderWriter
{
    public class BoolRowReaderWriter : RowReaderWriter
    {
        public BoolRowReaderWriter()
        {
            RowLength = 9;
        }

        public override void WriteRow(BinaryWriter writer, IDataRow row)
        {
            base.WriteRow(writer, row);
            writer.Write(Convert.ToBoolean(row.Value));
        }

        public override ISingleDataRow<T> ReadRow<T>(BinaryReader reader)
        {
            var row = new SingleDataRow<T>(ReadDate(reader), (T)Convert.ChangeType(reader.ReadBoolean(), typeof(T)));
            return row;
        }

        public override IDataRow ReadRow(BinaryReader reader)
        {
            return new DataRow { Key = ReadDate(reader), Value = reader.ReadBoolean() };
        }
    }
}
using System;
using System.IO;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace FileDb.RowReaderWriter
{
    public class DecimalRowReaderWriter : RowReaderWriter
    {
        public DecimalRowReaderWriter()
        {
            RowLength = 24;
        }

        public override void WriteRow(BinaryWriter writer, IDataRow row)
        {
            base.WriteRow(writer, row);
            writer.Write(Convert.ToDecimal(row.Value));
        }

        public override ISingleDataRow<T> ReadRow<T>(BinaryReader reader)
        {
            var row = new SingleDataRow<T>(ReadDate(reader), (T)Convert.ChangeType(reader.ReadDecimal(), typeof(T)));
            return row;
        }

        public override IDataRow ReadRow(BinaryReader reader)
        {
            return new DataRow { Key = ReadDate(reader), Value = reader.ReadDecimal() };
        }
    }
}
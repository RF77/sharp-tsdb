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
            writer.Write((float) Convert.ToSingle(row.Values[0]));
        }

        public override IDataRow ReadRow(BinaryReader reader)
        {
            var row = new FloatDataRow
            {
                Key = DateTime.FromBinary(reader.ReadInt64()),
                Value = reader.ReadSingle()
            };

            return row;
        }
    }
}
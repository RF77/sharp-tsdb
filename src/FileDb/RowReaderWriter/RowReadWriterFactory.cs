using System;
using FileDb.Impl;

namespace FileDb.RowReaderWriter
{
    public class RowReadWriterFactory
    {
        //Later, a more sophisticated factory method is required
        public RowReaderWriter CreateRowReaderWriter(MeasurementMetadata metadata)
        {
            var valueType = metadata.ColumnsInternal[1].ValueType;
            if (valueType == typeof(float))
            {
                return new FloatRowReaderWriter();
            }
            if (valueType == typeof(double))
            {
                return new DoubleRowReaderWriter();
            }
            if (valueType == typeof(bool))
            {
                return new BoolRowReaderWriter();
            }
            if (valueType == typeof(byte))
            {
                return new ByteRowReaderWriter();
            }
            if (valueType == typeof(short))
            {
                return new ShortRowReaderWriter();
            }
            if (valueType == typeof(int))
            {
                return new IntRowReaderWriter();
            }
            if (valueType == typeof(long))
            {
                return new LongRowReaderWriter();
            }
            if (valueType == typeof(decimal))
            {
                return new DecimalRowReaderWriter();
            }
            if (valueType == typeof(DateTime))
            {
                return new DateTimeRowReaderWriter();
            }

            throw new NotSupportedException("Invalid column type");
        }
    }
}
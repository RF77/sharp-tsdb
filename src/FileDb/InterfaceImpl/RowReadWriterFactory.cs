using System;

namespace FileDb.InterfaceImpl
{
    public class RowReadWriterFactory
    {
        //Later, a more sophisticated factory method is required
        public RowReaderWriter CreateRowReaderWriter(MeasurementMetadata metadata)
        {

            if (metadata.ColumnsInternal[1].ValueType == typeof (float))
            {
                return new FloatRowReaderWriter();
            }

            throw new NotSupportedException("Invalid column type");
        }
    }
}
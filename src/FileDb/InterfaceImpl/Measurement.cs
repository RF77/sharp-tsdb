using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using DbInterfaces.Interfaces;
using FileDb.Properties;

namespace FileDb.InterfaceImpl
{
    [DataContract]
    public class Measurement : IMeasurement
    {
        private static RowReadWriterFactory _rowReadWriterFactory;

        [DataMember]
        private readonly Db _db;

        public string BinaryFilePath { get; private set; }

        [DataMember]
        public MeasurementMetadata MetadataInternal { get; set; }

        public IMeasurementMetadata Metadata => MetadataInternal;

        private RowReaderWriter _rowReaderWriter;

        /// <summary>
        /// Only for deserialization
        /// </summary>
        private Measurement()
        {
            
        }

        public Measurement(MeasurementMetadata metadataInternal, Db db)
        {
            _db = db;
            MetadataInternal = metadataInternal;
            Init();
        }

        /// <summary>
        /// Creates a measurement with the specified name, DateTime as Key and the specified type as Value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="db"></param>
        public Measurement(string name, Type type, Db db)
        {
            _db = db;
            MetadataInternal = new MeasurementMetadata(name);
            MetadataInternal.ColumnsInternal.Add(new Column(Settings.Default.KeyColumnName, typeof(DateTime)));
            MetadataInternal.ColumnsInternal.Add(new Column(Settings.Default.ValueColumnName, type));
            Init();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext c)
        {
            Init();
        }

        private void Init()
        {
            BinaryFilePath = Path.Combine(_db.MeasurementDirectory, $"{MetadataInternal.Id}{Settings.Default.BinaryFileExtension}");
            if (!File.Exists(BinaryFilePath))
            {
                using (File.Create(BinaryFilePath))
                {
                    
                }
            }
            if (_rowReadWriterFactory == null)
            {
                _rowReadWriterFactory = new RowReadWriterFactory();
            }
            _rowReaderWriter = _rowReadWriterFactory.CreateRowReaderWriter(MetadataInternal);
        }

        public void AddDataPoints(IEnumerable<IDataRow> row)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDataRow> GetDataPoints(DateTime? @from, DateTime? to)
        {
            throw new NotImplementedException();
        }
    }
}
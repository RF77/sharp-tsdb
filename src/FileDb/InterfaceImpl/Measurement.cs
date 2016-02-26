using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using DbInterfaces.Interfaces;
using FileDb.Properties;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace FileDb.InterfaceImpl
{
    [DataContract]
    public class Measurement : ReadWritLockable, IMeasurement
    {
        private const int MinSearchRange = 1000;

        [DataMember] private readonly Db _db;
        private static RowReadWriterFactory _rowReadWriterFactory;

        private RowReaderWriter _rowReaderWriter;

        /// <summary>
        ///     Only for deserialization
        /// </summary>
        private Measurement():base(TimeSpan.FromMinutes(3))
        {
        }

        public Measurement(MeasurementMetadata metadataInternal, Db db):this()
        {
            _db = db;
            MetadataInternal = metadataInternal;
            Init();
        }

        /// <summary>
        ///     Creates a measurement with the specified name, DateTime as Key and the specified type as Value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="db"></param>
        public Measurement(string name, Type type, Db db):this()
        {
            _db = db;
            MetadataInternal = new MeasurementMetadata(name);
            MetadataInternal.ColumnsInternal.Add(new Column(Settings.Default.KeyColumnName, typeof (DateTime)));
            MetadataInternal.ColumnsInternal.Add(new Column(Settings.Default.ValueColumnName, type));
            Init();
        }

        [DataMember]
        public MeasurementMetadata MetadataInternal { get; set; }

        public string BinaryFilePath { get; private set; }

        public IQuerySerie<T> GetDataPoints<T>(string timeExpression) where T : struct
        {
            var expression = new TimeExpression(timeExpression);
            return GetDataPoints<T>(expression.From, expression.To);
        }

        public void ClearDataPoints(DateTime? after)
        {
            WriterLock(() =>
            {
                if (after == null)
                {
                    using (var fs = File.Open(BinaryFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                    }                    
                }
                else
                {
                    ClearDataPointsAfter(after.Value);
                }

            });
        }

        public IMeasurementMetadata Metadata => MetadataInternal;

        public void AppendDataPoints(IEnumerable<IDataRow> row)
        {
            WriterLock(() =>
            {
                using (var fs = File.Open(BinaryFilePath, FileMode.Append, FileAccess.Write, FileShare.None))
                {
                    using (var bw = new BinaryWriter(fs))
                    {
                        foreach (var dataRow in row)
                        {
                            _rowReaderWriter.WriteRow(bw, dataRow);
                        }
                    }
                }
            });
        }

        private void ClearDataPointsAfter(DateTime after) 
        {
                using (var fs = File.Open(BinaryFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var items = fs.Length / _rowReaderWriter.RowLength;
                    var currentItem = items / 2;

                    using (var binaryReader = new BinaryReader(fs))
                    {
                        var itemToStart = GetItemToStart(after, fs, binaryReader, currentItem, currentItem, 0);
                        if (itemToStart > 0)
                        {
                            itemToStart--;
                        }
                        fs.Position = itemToStart * _rowReaderWriter.RowLength;

                        while (fs.Position < fs.Length)
                        {
                            long position = fs.Position;
                            var readRow = _rowReaderWriter.ReadRow(binaryReader);

                            if (readRow.Key >= after)
                            {
                                fs.SetLength(position);
                                break;
                            }
                        }
                    }
                }
        }


        public IQuerySerie<T> GetDataPoints<T>(DateTime? @from = null, DateTime? to = null) where T : struct
        {
            return ReaderLock(() =>
            {
                var start = from ?? DateTime.MinValue;
                var stop = to ?? DateTime.MaxValue;

                using (var fs = File.Open(BinaryFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var items = fs.Length/_rowReaderWriter.RowLength;
                    var currentItem = items/2;

                    using (var binaryReader = new BinaryReader(fs))
                    {
                        var itemToStart = GetItemToStart(start, fs, binaryReader, currentItem, currentItem, 0);
                        if (itemToStart > 0)
                        {
                            itemToStart--;
                        }
                        fs.Position = itemToStart*_rowReaderWriter.RowLength;

                        return ReadRows<T>(fs, binaryReader, start, stop, from, to);
                    }
                }
            });
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext c)
        {
            Init();
        }

        private void Init()
        {
            BinaryFilePath = Path.Combine(_db.MeasurementDirectory,
                $"{MetadataInternal.Id.First()}\\{MetadataInternal.Id}{Settings.Default.BinaryFileExtension}");
            var fileInfo = new FileInfo(BinaryFilePath);
            if (!fileInfo.Directory?.Exists ?? false)
            {
                fileInfo.Directory?.Create();
            }
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

        private IQuerySerie<T> ReadRows<T>(FileStream fs, BinaryReader binaryReader, DateTime start,
            DateTime stop, DateTime? @from = null, DateTime? to = null) where T : struct
        {
            ISingleDataRow<T> firstRow = null;
            var rows = new List<ISingleDataRow<T>>();
            var data = new QuerySerie<T>(rows, from, to);

            while (fs.Position < fs.Length)
            {
                var readRow = _rowReaderWriter.ReadRow<T>(binaryReader);

                if (readRow.Time >= start)
                {
                    if (readRow.Time <= stop)
                    {
                        rows.Add(readRow);
                    }
                }
                else
                {
                    firstRow = readRow;
                }

                if (readRow.Time >= stop)
                {
                    data.NextRow = readRow;
                    break;
                }
            }

            data.Name = Metadata.Name;
            data.PreviousRow = firstRow;

            return data;
        }

        private long GetItemToStart(DateTime start, FileStream fs, BinaryReader binaryReader, long currentIndex,
            long currentRange, long lastValidIndex)
        {
            if (currentRange < MinSearchRange)
            {
                return lastValidIndex;
            }
            fs.Position = currentIndex*_rowReaderWriter.RowLength;
            var time = DateTime.FromBinary(binaryReader.ReadInt64());
            currentRange = currentRange/2;
            if (time >= start)
            {
                currentIndex = currentIndex - currentRange;
            }
            else
            {
                lastValidIndex = Math.Max(currentIndex, 0);
                currentIndex = lastValidIndex + currentRange;
            }

            return GetItemToStart(start, fs, binaryReader, currentIndex, currentRange, lastValidIndex);
        }
    }
}
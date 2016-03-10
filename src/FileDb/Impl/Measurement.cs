// /*******************************************************************************
//  * Copyright (c) 2016 by RF77 (https://github.com/RF77)
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the Eclipse Public License v1.0
//  * which accompanies this distribution, and is available at
//  * http://www.eclipse.org/legal/epl-v10.html
//  *
//  * Contributors:
//  *    RF77 - initial API and implementation and/or initial documentation
//  *******************************************************************************/ 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using DbInterfaces.Interfaces;
using FileDb.Properties;
using FileDb.RowReaderWriter;
using Timeenator.Extensions;
using Timeenator.Extensions.Converting;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace FileDb.Impl
{
    [DebuggerDisplay("{MetadataInternal.Name}")]
    [DataContract]
    public class Measurement : ReadWritLockable, IMeasurement
    {
        private const int MinSearchRange = 1000;
        private static RowReadWriterFactory _rowReadWriterFactory;

        [DataMember] private readonly Db _db;

        private IDataRow _currentValue;
        private RowReaderWriter.RowReaderWriter _rowReaderWriter;

        /// <summary>
        ///     Only for deserialization
        /// </summary>
        private Measurement() : base(TimeSpan.FromMinutes(3))
        {
        }

        public Measurement(MeasurementMetadata metadataInternal, Db db) : this()
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
        public Measurement(string name, Type type, Db db) : this()
        {
            _db = db;
            MetadataInternal = new MeasurementMetadata(name);
            MetadataInternal.ColumnsInternal.Add(new Column(Settings.Default.KeyColumnName, typeof (DateTime)));
            MetadataInternal.ColumnsInternal.Add(new Column(Settings.Default.ValueColumnName, type));
            Init();
        }

        [DataMember]
        public MeasurementMetadata MetadataInternal { get; set; }

        public Type ValueType => _rowReaderWriter.ValueType;
        public string BinaryFilePath { get; private set; }

        public IQuerySerie<T> GetDataPoints<T>(string timeExpression) where T : struct
        {
            if (timeExpression != null)
            {
                var expression = new TimeExpression(timeExpression);
                return GetDataPoints<T>(expression.From, expression.To);
            }
            return GetDataPoints<T>();
        }

        public ISingleDataRow<T> CurrentValue<T>() where T : struct
        {
            return ReaderLock(() =>
            {
                if (_currentValue != null)
                {
                    return new SingleDataRow<T>(_currentValue.Key, _currentValue.Value.ToType<T>());
                }
                return null;
            });
        }

        public DateTime? FirstValueTimeUtc { get; set; }
        public long Size { get; set; }
        public string Name => MetadataInternal.Name;
        public long NumberOfItems { get; set; }

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

        public IEnumerable<string> NameAndAliases => MetadataInternal.Aliases.Concat(new[] {MetadataInternal.Name});

        public IMeasurementMetadata Metadata => MetadataInternal;

        public void MergeDataPoints(IEnumerable<IDataRow> rows, Func<IEnumerable<IDataRow>, IEnumerable<IDataRow>> mergeFunc)
        {
            var newRows = rows.OrderBy(i => i.Key).ToList();
            if (!newRows.Any()) return;
            WriterLock(() =>
            {
                var startDate = newRows.First().Key;
                var existingRows = GetDataPoints(startDate);
                if (!existingRows.Rows.Any())
                {
                    AppendDataPoints(mergeFunc(newRows));
                }
                else
                {
                    var mergedRows = mergeFunc(existingRows.Rows.Concat(newRows).OrderBy(i => i.Key));
                    ClearDataPointsAfter(startDate);
                    AppendDataPoints(mergedRows);
                }
            });
        }


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

        public IQuerySerie<T> GetDataPoints<T>(DateTime? @from = null, DateTime? to = null) where T : struct
        {
            //TODO: optimize duplicated code with GetDataPoints
            return ReaderLock(() =>
            {
                var start = from?.ToUniversalTime() ?? DateTime.MinValue;
                var stop = to?.ToUniversalTime() ?? DateTime.MaxValue;

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

        public IObjectQuerySerie GetDataPoints(DateTime? @from = null, DateTime? to = null)
        {
            //TODO: optimize duplicated code with GetDataPoints<T>
            return ReaderLock(() =>
            {
                var start = from?.ToUniversalTime() ?? DateTime.MinValue;
                var stop = to?.ToUniversalTime() ?? DateTime.MaxValue;

                using (var fs = File.Open(BinaryFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var items = fs.Length / _rowReaderWriter.RowLength;
                    var currentItem = items / 2;

                    using (var binaryReader = new BinaryReader(fs))
                    {
                        var itemToStart = GetItemToStart(start, fs, binaryReader, currentItem, currentItem, 0);
                        if (itemToStart > 0)
                        {
                            itemToStart--;
                        }
                        fs.Position = itemToStart * _rowReaderWriter.RowLength;

                        return ReadRows(fs, binaryReader, start, stop, from, to);
                    }
                }
            });
        }

        private void ClearDataPointsAfter(DateTime after)
        {
            using (var fs = File.Open(BinaryFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                var items = fs.Length/_rowReaderWriter.RowLength;
                var currentItem = items/2;

                using (var binaryReader = new BinaryReader(fs))
                {
                    var itemToStart = GetItemToStart(after, fs, binaryReader, currentItem, currentItem, 0);
                    if (itemToStart > 0)
                    {
                        itemToStart--;
                    }
                    fs.Position = itemToStart*_rowReaderWriter.RowLength;

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
            InitCurrentValues();

        }

        public void InitCurrentValues()
        {
            var fileInfo = new FileInfo(BinaryFilePath);
            Size = fileInfo.Length;
            NumberOfItems = Size/_rowReaderWriter.RowLength;
            if (NumberOfItems > 0)
            {
                using (var fs = File.Open(BinaryFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var binaryReader = new BinaryReader(fs))
                    {
                        FirstValueTimeUtc = _rowReaderWriter.ReadRow(binaryReader).Key;
                        fs.Position = Size - _rowReaderWriter.RowLength;
                        _currentValue = _rowReaderWriter.ReadRow(binaryReader);
                    }
                }
            }
        }

        private IQuerySerie<T> ReadRows<T>(FileStream fs, BinaryReader binaryReader, DateTime start,
            DateTime stop, DateTime? @from = null, DateTime? to = null) where T : struct
        {
            //TODO: optimize duplicated code with ReadRows
            ISingleDataRow<T> firstRow = null;
            var rows = new List<ISingleDataRow<T>>();
            var data = new QuerySerie<T>(rows, from, to);

            while (fs.Position < fs.Length)
            {
                var readRow = _rowReaderWriter.ReadRow<T>(binaryReader);

                if (readRow.TimeUtc >= start)
                {
                    if (readRow.TimeUtc <= stop)
                    {
                        rows.Add(readRow);
                    }
                }
                else
                {
                    firstRow = readRow;
                }

                if (readRow.TimeUtc >= stop)
                {
                    data.NextRow = readRow;
                    break;
                }
            }

            data.Name = Metadata.Name;
            data.PreviousRow = firstRow;

            return data;
        }

        private IObjectQuerySerie ReadRows(FileStream fs, BinaryReader binaryReader, DateTime start,
            DateTime stop, DateTime? @from = null, DateTime? to = null)
        {
            //TODO: optimize duplicated code with ReadRows<T>
            IObjectSingleDataRow firstRow = null;
            var rows = new List<IObjectSingleDataRow>();
            var data = new ObjectQuerySerie(rows, from, to);

            while (fs.Position < fs.Length)
            {
                var readRow = _rowReaderWriter.ReadRow(binaryReader);

                if (readRow.TimeUtc >= start)
                {
                    if (readRow.TimeUtc <= stop)
                    {
                        rows.Add(readRow);
                    }
                }
                else
                {
                    firstRow = readRow;
                }

                if (readRow.TimeUtc >= stop)
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

        public void AddAlias(string @alias)
        {
            MetadataInternal.Aliases.Add(alias);
            _db.MetadataInternal.MeasurementsWithAliases[alias] = this;
        }
    }
}
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
using DbInterfaces.Interfaces;
using FileDb.Impl;
using FileDb.Scripting;
using FluentAssertions;
using MathNet.Numerics.Statistics;
using NUnit.Framework;
using Timeenator.Extensions.Grouping;
using Timeenator.Extensions.Rows;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace Tests.FileDb
{
    [TestFixture]
    public class DbTests
    {
        [SetUp]
        public void Setup()
        {
            _dbm = new DbManagement(true);
            _dbm.DetachAllDbs();
            _unitUnderTest = (Db) _dbm.CreateDb(DbRootDir, TestDbName);
            _unitUnderTest.DeleteAllMeasurements();
        }

        [TearDown]
        public void TearDown()
        {
            _dbm.DeleteDb(_unitUnderTest.Name);
        }

        private const string DbRootDir = @"c:\temp\DBs";
        private const string TestDbName = "test_TestDb";
        private const string TestMeasName = "test_Meas";
        private const string TestMeas2Name = "test_Meas2";
        private Db _unitUnderTest;
        private DbManagement _dbm;

        private IEnumerable<IDataRow> CreateFloatRows(int numberOfRows)
        {
            DateTime time = new DateTime(2000, 1, 1, 0, 0, 33, DateTimeKind.Utc);
            for (int i = 0; i < numberOfRows; i++)
            {
                var row = new SingleDataRow<float>(time, (float) (i*0.5));
                time += TimeSpan.FromMinutes(1);
                yield return row;
            }
        }

        [Test]
        public void Add10Items()
        {
            var measurement = _unitUnderTest.CreateMeasurement(TestMeasName, typeof (float));
            var numberOfRows = 10;
            measurement.AppendDataPoints(CreateFloatRows(numberOfRows));

            File.Exists(measurement.BinaryFilePath).Should().BeTrue();
            new FileInfo(measurement.BinaryFilePath).Length.Should()
                .Be(measurement.Metadata.Columns.Sum(i => i.Size)*numberOfRows);

            measurement.AppendDataPoints(CreateFloatRows(numberOfRows));

            File.Exists(measurement.BinaryFilePath).Should().BeTrue();
            new FileInfo(measurement.BinaryFilePath).Length.Should()
                .Be(measurement.Metadata.Columns.Sum(i => i.Size)*numberOfRows*2);
        }

        [Test]
        public void ClearItems()
        {
            var measurement = _unitUnderTest.CreateMeasurement(TestMeasName, typeof (float));
            var numberOfRows = 10;
            measurement.AppendDataPoints(CreateFloatRows(numberOfRows));

            File.Exists(measurement.BinaryFilePath).Should().BeTrue();
            new FileInfo(measurement.BinaryFilePath).Length.Should()
                .Be(measurement.Metadata.Columns.Sum(i => i.Size)*numberOfRows);

            measurement.ClearDataPoints();

            File.Exists(measurement.BinaryFilePath).Should().BeTrue();
            new FileInfo(measurement.BinaryFilePath).Length.Should().Be(0);
        }

        [Test]
        public void ClearItemsAfterDate()
        {
            var measurement = _unitUnderTest.CreateMeasurement(TestMeasName, typeof (float));
            var numberOfRows = 10000;
            measurement.AppendDataPoints(CreateFloatRows(numberOfRows));

            File.Exists(measurement.BinaryFilePath).Should().BeTrue();
            new FileInfo(measurement.BinaryFilePath).Length.Should()
                .Be(measurement.Metadata.Columns.Sum(i => i.Size)*numberOfRows);

            measurement.ClearDataPoints(new DateTime(2010, 1, 2, 0, 1, 0, DateTimeKind.Utc));

            var serie = measurement.GetDataPoints<float>();
            serie.Rows.Count.Should().Be(numberOfRows);

            var after = new DateTime(2000, 1, 2, 0, 1, 0, DateTimeKind.Utc);
            measurement.ClearDataPoints(after);
            serie = measurement.GetDataPoints<float>();
            var last = serie.Rows.Last();
            last.TimeUtc.Should().BeBefore(after);

            measurement.ClearDataPoints(new DateTime(200, 1, 2, 0, 1, 0, DateTimeKind.Utc));
            serie = measurement.GetDataPoints<float>();
            serie.Rows.Count.Should().Be(0);
        }

        [Test]
        public void CreateMeasurement()
        {
            _unitUnderTest.CreateMeasurement(TestMeasName, typeof (float));

            var measurement = _unitUnderTest.GetMeasurement(TestMeasName);
            measurement.Metadata.Name.Should().Be(TestMeasName);
            File.Exists(measurement.BinaryFilePath).Should().BeTrue();
            _unitUnderTest.GetMeasurementNames().Count.Should().Be(1);
            _unitUnderTest.GetMeasurementNames().First().Should().Be(TestMeasName);
        }

        [Test]
        public void MeasurementExistingInSecondInstance()
        {
            _unitUnderTest.CreateMeasurement(TestMeasName, typeof (float));

            var measurement = _unitUnderTest.GetMeasurement(TestMeasName);
            var binName = measurement.BinaryFilePath;

            var dbm = new DbManagement(true);
            var db = dbm.GetDb(TestDbName);
            var m = db.GetMeasurement(TestMeasName);
            m.BinaryFilePath.Should().Be(binName);
        }

        [Test]
        public void ReadItems()
        {
            var measurement = _unitUnderTest.CreateMeasurement(TestMeasName, typeof (float));
            var numberOfRows = 100000;
            measurement.AppendDataPoints(CreateFloatRows(numberOfRows));

            //All (without dates)
            var data = measurement.GetDataPoints<float>();
            var allItems = data.Rows;
            allItems.Count.Should().Be(numberOfRows);
            data.StartTime.Should().Be(null);
            data.EndTime.Should().Be(null);

            var dateTime2000 = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            data = measurement.GetDataPoints<float>(dateTime2000);
            allItems = data.Rows;
            allItems.Count.Should().Be(numberOfRows);
            data.StartTime.Should().Be(dateTime2000);
            data.EndTime.Should().Be(null);

            var dateTime2200 = new DateTime(2200, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            data = measurement.GetDataPoints<float>(dateTime2000, dateTime2200);
            allItems = data.Rows;
            allItems.Count.Should().Be(numberOfRows);
            data.StartTime.Should().Be(dateTime2000);
            data.EndTime.Should().Be(dateTime2200);

            var singleItems = measurement.GetDataPoints<float>(dateTime2000, dateTime2200).Rows;
            singleItems.Count.Should().Be(allItems.Count());

            var time = Stopwatch.StartNew();
            var dateTimeStart = new DateTime(2000, 1, 7, 0, 0, 0, DateTimeKind.Utc);
            var dateTime200012 = new DateTime(2000, 1, 21, 0, 0, 0, DateTimeKind.Utc);

            //From begin to middle
            data = measurement.GetDataPoints<float>(null, dateTime200012);
            var items = data.Rows;
            items[0].TimeUtc.Should().BeAfter(dateTime2000);
            items.Last().TimeUtc.Should().BeOnOrBefore(dateTime200012);
            data.PreviousRow.Should().Be(null);
            data.NextRow.TimeUtc.Should().BeAfter(dateTime200012);
            time.Stop();

            //From middle to end
            data = measurement.GetDataPoints<float>(dateTime200012, dateTime2200);
            items = data.Rows;
            items[0].TimeUtc.Should().BeOnOrAfter(dateTime200012);
            items.Last().TimeUtc.Should().BeOnOrBefore(dateTime2200);
            data.PreviousRow.TimeUtc.Should().BeBefore(dateTime200012);
            data.NextRow.Should().Be(null);

            time = Stopwatch.StartNew();


            //From middle to middle
            data = measurement.GetDataPoints<float>(dateTimeStart, dateTime200012);
            items = data.Rows;
            items[0].TimeUtc.Should().BeOnOrAfter(dateTimeStart);
            items.Last().TimeUtc.Should().BeOnOrBefore(dateTime200012);
            data.PreviousRow.TimeUtc.Should().BeBefore(dateTimeStart);
            data.NextRow.TimeUtc.Should().BeAfter(dateTime200012);

            time.Stop();
        }

        [Test]
        public void ReadItemsAndConvert()
        {
            var measurement = _unitUnderTest.CreateMeasurement(TestMeasName, typeof (float));
            var numberOfRows = 100000;
            measurement.AppendDataPoints(CreateFloatRows(numberOfRows));

            //All (without dates)
            var rows = measurement.GetDataPoints<double>();
            var allItems = rows.GroupByMinutes(5, a => a.Values.HarmonicMean()).Rows.ToArray();
        }

        [Test]
        public void ReadItemsPerScripting()
        {
            var measurement = _unitUnderTest.CreateMeasurement(TestMeasName, typeof (float));
            var numberOfRows = 100000;
            measurement.AppendDataPoints(CreateFloatRows(numberOfRows));

            var time = Stopwatch.StartNew();

            //All (without dates)
            var scriptingEngine = new ScriptingEngine(_unitUnderTest, $@"db.GetSerie<float>(""{TestMeasName}"", null)");
            var data = scriptingEngine.Execute().Result;
            var allItems = data.Series.First().Rows;
            allItems.Count.Should().Be(numberOfRows);
            data.Series.First().StartTime.Should().Be(null);
            data.Series.First().EndTime.Should().Be(null);

            //var dateTime2000 = new DateTime(2000, 1, 1);
            //data = measurement.GetDataPoints<float>(dateTime2000);
            //allItems = data.Rows;
            //allItems.Count().Should().Be(numberOfRows);
            //data.StartTime.Should().Be(dateTime2000);
            //data.EndTime.Should().Be(null);

            //var dateTime2200 = new DateTime(2200, 1, 1);
            //data = measurement.GetDataPoints<float>(dateTime2000, dateTime2200);
            //allItems = data.Rows;
            //allItems.Count().Should().Be(numberOfRows);
            //data.StartTime.Should().Be(dateTime2000);
            //data.EndTime.Should().Be(dateTime2200);

            //var singleItems = measurement.GetDataPoints<float>(dateTime2000, dateTime2200).Rows;
            //singleItems.Count().Should().Be(allItems.Count());

            //var time = Stopwatch.StartNew();
            //var dateTimeStart = new DateTime(2000, 1, 7);
            //var dateTime200012 = new DateTime(2000, 1, 21);

            ////From begin to middle
            //data = measurement.GetDataPoints<float>(null, dateTime200012);
            //var items = data.Rows;
            //items[0].Key.Should().BeAfter(dateTime2000);
            //items.Last().Key.Should().BeOnOrBefore(dateTime200012);
            //data.PreviousRow.Should().Be(null);
            //data.NextRow.Key.Should().BeAfter(dateTime200012);
            //time.Stop();

            ////From middle to end
            //data = measurement.GetDataPoints<float>(dateTime200012, dateTime2200);
            //items = data.Rows;
            //items[0].Key.Should().BeOnOrAfter(dateTime200012);
            //items.Last().Key.Should().BeOnOrBefore(dateTime2200);
            //data.PreviousRow.Key.Should().BeBefore(dateTime200012);
            //data.NextRow.Should().Be(null);

            //time = Stopwatch.StartNew();


            ////From middle to middle
            //data = measurement.GetDataPoints<float>(dateTimeStart, dateTime200012);
            //items = data.Rows;
            //items[0].Key.Should().BeOnOrAfter(dateTimeStart);
            //items.Last().Key.Should().BeOnOrBefore(dateTime200012);
            //data.PreviousRow.Key.Should().BeBefore(dateTimeStart);
            //data.NextRow.Key.Should().BeAfter(dateTime200012);

            time.Stop();
        }

        [Test]
        public void AliasAndDeleteTests()
        {
            var measurement = _unitUnderTest.GetOrCreateMeasurement("XyInt", typeof (int));
            measurement.ValueType.Should().Be(typeof (int));
            _unitUnderTest.AddAliasToMeasurements("XyInt", "ABC");
            _unitUnderTest.MetadataInternal.Measurements.Count.Should().Be(1);
            _unitUnderTest.MetadataInternal.MeasurementsWithAliases.Count.Should().Be(2);
            var aliasMeasurment = _unitUnderTest.GetOrCreateMeasurement("ABC");
            measurement.Should().Be(aliasMeasurment);
        }

        [Test]
        public void MergeOnlyValueChangesTest()
        {
            var rowsA = new[]
            {
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc),  1),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 1, DateTimeKind.Utc),  1),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 2, DateTimeKind.Utc),  3),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 5, DateTimeKind.Utc),  4),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 8, DateTimeKind.Utc),  5),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 9, DateTimeKind.Utc),  6),
            };

            var rowsB = new[]
            {
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 9, DateTimeKind.Utc), 6),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 2, DateTimeKind.Utc), 3),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 3, DateTimeKind.Utc), 3),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 4, DateTimeKind.Utc), 4),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 5, DateTimeKind.Utc), 4),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 8, DateTimeKind.Utc), 5),
            };

            _unitUnderTest.GetOrCreateMeasurement("A").AppendDataPoints(rowsA);
            _unitUnderTest.GetOrCreateMeasurement("A").MergeDataPoints(rowsB, o => o.ValueChanges());

            var result = _unitUnderTest.GetSerie<float>("A");

            result.Rows[0].Value.Should().Be(1);
            result.Rows[1].Value.Should().Be(1);
            result.Rows[3].Key.Should().Be(new DateTime(2010, 1, 1, 0, 0, 4, DateTimeKind.Utc));
            result.Rows.Count.Should().Be(6);
        }

        [Test]
        public void MergeTimeSpanTest()
        {
            var rowsA = new[]
            {
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc),  1),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 1, DateTimeKind.Utc),  1),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 2, DateTimeKind.Utc),  3),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 5, DateTimeKind.Utc),  4),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 8, DateTimeKind.Utc),  5),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 9, DateTimeKind.Utc),  6),
            };

            var rowsB = new[]
            {
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 9, DateTimeKind.Utc), 6),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 2, DateTimeKind.Utc), 3),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 3, DateTimeKind.Utc), 3),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 4, DateTimeKind.Utc), 4),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 5, DateTimeKind.Utc), 4),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 8, DateTimeKind.Utc), 5),
            };

            _unitUnderTest.GetOrCreateMeasurement("A").AppendDataPoints(rowsA);
            _unitUnderTest.GetOrCreateMeasurement("A").MergeDataPoints(rowsB, o => o.MinimalTimeSpan("2s"));

            var result = _unitUnderTest.GetSerie<float>("A");

            result.Rows.Count.Should().Be(5);
        }

        [Test]
        public void MergeTimeSpanAfterTest()
        {
            var rowsA = new[]
            {
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc),  1),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 1, DateTimeKind.Utc),  1),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 2, DateTimeKind.Utc),  3),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 5, DateTimeKind.Utc),  4),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 8, DateTimeKind.Utc),  5),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 9, DateTimeKind.Utc),  6),
            };

            var rowsB = new[]
            {
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 1, 0, 9, DateTimeKind.Utc), 6),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 1, 0, 2, DateTimeKind.Utc), 3),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 1, 0, 3, DateTimeKind.Utc), 3),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 1, 0, 4, DateTimeKind.Utc), 4),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 1, 0, 5, DateTimeKind.Utc), 4),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 1, 0, 8, DateTimeKind.Utc), 5),
            };

            _unitUnderTest.GetOrCreateMeasurement("A").AppendDataPoints(rowsA);
            _unitUnderTest.GetOrCreateMeasurement("A").MergeDataPoints(rowsB, o => o.MinimalTimeSpan("2s"));

            var result = _unitUnderTest.GetSerie<float>("A");

            result.Rows.Count.Should().Be(9);
        }

        [Test]
        public void MergeTimeSpanBeforeTest()
        {
            var rowsA = new[]
            {
                new SingleDataRow<float>(new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Utc),  1),
                new SingleDataRow<float>(new DateTime(2011, 1, 1, 0, 0, 1, DateTimeKind.Utc),  1),
                new SingleDataRow<float>(new DateTime(2011, 1, 1, 0, 0, 2, DateTimeKind.Utc),  3),
                new SingleDataRow<float>(new DateTime(2011, 1, 1, 0, 0, 5, DateTimeKind.Utc),  4),
                new SingleDataRow<float>(new DateTime(2011, 1, 1, 0, 0, 8, DateTimeKind.Utc),  5),
                new SingleDataRow<float>(new DateTime(2011, 1, 1, 0, 0, 9, DateTimeKind.Utc),  6),
            };

            var rowsB = new[]
            {
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 1, 0, 9, DateTimeKind.Utc), 6),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 1, 0, 2, DateTimeKind.Utc), 3),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 1, 0, 3, DateTimeKind.Utc), 3),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 1, 0, 4, DateTimeKind.Utc), 4),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 1, 0, 5, DateTimeKind.Utc), 4),
                new SingleDataRow<float>(new DateTime(2010, 1, 1, 1, 0, 8, DateTimeKind.Utc), 5),
            };

            _unitUnderTest.GetOrCreateMeasurement("A").AppendDataPoints(rowsA);
            _unitUnderTest.GetOrCreateMeasurement("A").MergeDataPoints(rowsB, o => o.MinimalTimeSpan("2s"));

            var result = _unitUnderTest.GetSerie<float>("A");

            result.Rows.Count.Should().Be(7);
        }

        [Test, Ignore]
        public void RecreateFuxDb()
        {
            var dbm = new DbManagement();
            var dbName = "fux";
            if (dbm.GetDbNames().Contains(dbName))
            {
                dbm.DeleteDb(dbName);
            }
            dbm.CreateDb(@"c:\DBs\SharpTsdb", dbName);
        }

        [Test, Ignore]
        public void SortTest()
        {
            var sourceList = Enumerable.Range(0, 1000000).Reverse().ToList();
            var sw = Stopwatch.StartNew();
            var newList = sourceList.OrderBy(i => i).ToList();
            //sourceList.Sort();
            sw.Stop();
        }

        [Test, Ignore]
        public void GetMeasurementsTest()
        {
            var dbm = new DbManagement();
            var db = dbm.GetDb("Haus");
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                var ms = db.GetMeasurements("a");
            }
            sw.Stop();
        }

        [Test, Ignore]
        public void AllMeasurementNamesTest()
        {
            var dbm = new DbManagement();
            var db = dbm.GetDb("Haus");
            var sw = Stopwatch.StartNew();
            var measurements = db.GetMeasurements(".*");
            var names = measurements.SelectMany(i => i.NameAndAliases).OrderBy(i => i).ToList();
            var nameString = string.Join("\r\n", names);
            var measurement = db.GetMeasurement("HmWetterstationSunshineduration");
            sw.Stop();
        }

        [Test, Ignore]
        public void AddAliasesTest()
        {
            var dbm = new DbManagement();
            var db = dbm.GetDb("Haus");
            var sw = Stopwatch.StartNew();
            db.AddAliasToMeasurements("OpenHAB.out.(.*).state", "$1");
            sw.Stop();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using FluentAssertions;
using MathNet.Numerics.Statistics;
using NUnit.Framework;
using QueryLanguage.Grouping;

namespace Tests.FileDb
{
    [TestFixture]
    public class DbTests
    {
        private const string DbRootDir = @"c:\temp\DBs";
        private const string TestDbName = "test_TestDb";
        private const string TestMeasName = "test_Meas";
        private const string TestMeas2Name = "test_Meas2";
        private Db _unitUnderTest;
        private DbManagement _dbm;

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

        [Test]
        public void CreateMeasurement()
        {
            _unitUnderTest.CreateMeasurement(TestMeasName, typeof(float));

            var measurement = _unitUnderTest.GetMeasurement(TestMeasName);
            measurement.Metadata.Name.Should().Be(TestMeasName);
            File.Exists(measurement.BinaryFilePath).Should().BeTrue();
            _unitUnderTest.GetMeasurementNames().Count.Should().Be(1);
            _unitUnderTest.GetMeasurementNames().First().Should().Be(TestMeasName);
        }

        [Test]
        public void MeasurementExistingInSecondInstance()
        {
            _unitUnderTest.CreateMeasurement(TestMeasName, typeof(float));

            var measurement = _unitUnderTest.GetMeasurement(TestMeasName);
            var binName = measurement.BinaryFilePath;

            var dbm = new DbManagement(true);
            var db = dbm.GetDb(TestDbName);
            var m = db.GetMeasurement(TestMeasName);
            m.BinaryFilePath.Should().Be(binName);
        }

        [Test]
        public void Add10Items()
        {
            var measurement = _unitUnderTest.CreateMeasurement(TestMeasName, typeof(float));
            var numberOfRows = 10;
            measurement.AppendDataPoints(CreateFloatRows(numberOfRows));

            File.Exists(measurement.BinaryFilePath).Should().BeTrue();
            new FileInfo(measurement.BinaryFilePath).Length.Should()
                .Be(measurement.Metadata.Columns.Sum(i => i.Size)*numberOfRows);

            measurement.AppendDataPoints(CreateFloatRows(numberOfRows));

            File.Exists(measurement.BinaryFilePath).Should().BeTrue();
            new FileInfo(measurement.BinaryFilePath).Length.Should()
                .Be(measurement.Metadata.Columns.Sum(i => i.Size) * numberOfRows * 2);
        }

        IEnumerable<IDataRow> CreateFloatRows(int numberOfRows)
        {
            DateTime time = new DateTime(2000,1,1);
            for (int i = 0; i < numberOfRows; i++)
            {
                var row = new SingleDataRow<float>(time,(float)(i * 0.5));
                time += TimeSpan.FromMinutes(1);
                yield return row;
            }
        }

        [Test]
        public void ClearItems()
        {
            var measurement = _unitUnderTest.CreateMeasurement(TestMeasName, typeof(float));
            var numberOfRows = 10;
            measurement.AppendDataPoints(CreateFloatRows(numberOfRows));

            File.Exists(measurement.BinaryFilePath).Should().BeTrue();
            new FileInfo(measurement.BinaryFilePath).Length.Should()
                .Be(measurement.Metadata.Columns.Sum(i => i.Size) * numberOfRows);

            measurement.ClearDataPoints();

            File.Exists(measurement.BinaryFilePath).Should().BeTrue();
            new FileInfo(measurement.BinaryFilePath).Length.Should().Be(0);

        }

        [Test]
        public void ReadItems()
        {
            var measurement = _unitUnderTest.CreateMeasurement(TestMeasName, typeof(float));
            var numberOfRows = 100000;
            measurement.AppendDataPoints(CreateFloatRows(numberOfRows));

            //All (without dates)
            var allItems = measurement.GetDataPoints<float>().Rows;
            allItems.Count().Should().Be(numberOfRows);

            var dateTime1999 = new DateTime(1999, 1, 1);
            allItems = measurement.GetDataPoints<float>(dateTime1999).Rows;
            allItems.Count().Should().Be(numberOfRows);

            var dateTime2200 = new DateTime(2200,1,1);
            allItems = measurement.GetDataPoints<float>(dateTime1999, dateTime2200).Rows;
            allItems.Count().Should().Be(numberOfRows);

            var singleItems = measurement.GetDataPoints<float>(dateTime1999, dateTime2200).Rows;

            singleItems.Count().Should().Be(allItems.Count());

            var time = Stopwatch.StartNew();
            var dateTimeStart = new DateTime(2000, 1, 7);
            var dateTime200012 = new DateTime(2000, 1, 21);

            //From begin to middle
            var items = measurement.GetDataPoints<float>(dateTime1999, dateTime200012).Rows.ToList();
            items[0].Key.Should().BeAfter(dateTime1999);
            items.Last().Key.Should().BeOnOrBefore(dateTime200012);

            time.Stop();

            //From middle to end
            items = measurement.GetDataPoints<float>(dateTime200012, dateTime2200).Rows.ToList();
            items[0].Key.Should().BeBefore(dateTime200012);
            items[1].Key.Should().BeOnOrAfter(dateTime200012);
            items.Last().Key.Should().BeOnOrBefore(dateTime2200);

            time = Stopwatch.StartNew();


            //From middle to middle
            items = measurement.GetDataPoints<float>(dateTimeStart, dateTime200012).Rows.ToList();
            items[0].Key.Should().BeBefore(dateTimeStart);
            items[1].Key.Should().BeOnOrAfter(dateTimeStart);
            items.Last().Key.Should().BeOnOrBefore(dateTime200012);

            time.Stop();

        }

        [Test]
        public void ReadItemsAndConvert()
        {
            var measurement = _unitUnderTest.CreateMeasurement(TestMeasName, typeof(float));
            var numberOfRows = 100000;
            measurement.AppendDataPoints(CreateFloatRows(numberOfRows));

            //All (without dates)
            var rows = measurement.GetDataPoints<double>();
            var allItems = rows.GroupByMinutes(5, a => a.Values.HarmonicMean()).ToArray();


        }

    }
}

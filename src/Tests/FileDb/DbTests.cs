using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using FluentAssertions;
using NUnit.Framework;

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

        [TestCase]
        public void CreateMeasurement()
        {
            _unitUnderTest.CreateMeasurement(TestMeasName, typeof(float));

            var measurement = _unitUnderTest.GetMeasurement(TestMeasName);
            measurement.Metadata.Name.Should().Be(TestMeasName);
            File.Exists(measurement.BinaryFilePath).Should().BeTrue();
            _unitUnderTest.GetMeasurementNames().Count.Should().Be(1);
            _unitUnderTest.GetMeasurementNames().First().Should().Be(TestMeasName);
        }

        [TestCase]
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

        [TestCase]
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
                var row = new FloatDataRow()
                {
                    Key = time,
                    Value = (float)(i * 0.5)
                };
                time += TimeSpan.FromMinutes(1);
                yield return row;
            }
        }

    }
}

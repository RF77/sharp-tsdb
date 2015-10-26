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

        [SetUp]
        public void Setup()
        {
            var dbm = new DbManagement(true);
            dbm.DetachAllDbs();
            _unitUnderTest = (Db) dbm.CreateDb(DbRootDir, TestDbName);
            _unitUnderTest.DeleteAllMeasurements();
        }

        [TestCase]
        public void CreateMeasurement()
        {
            _unitUnderTest.CreateMeasurement(TestMeasName, typeof(long));

            var measurement = _unitUnderTest.GetMeasurement(TestMeasName);
            measurement.Metadata.Name.Should().Be(TestMeasName);
            File.Exists(measurement.BinaryFilePath).Should().BeTrue();
            _unitUnderTest.GetMeasurementNames().Count.Should().Be(1);
            _unitUnderTest.GetMeasurementNames().First().Should().Be(TestMeasName);
        }

        [TestCase]
        public void MeasurementExistingInSecondInstance()
        {
            _unitUnderTest.CreateMeasurement(TestMeasName, typeof(long));

            var measurement = _unitUnderTest.GetMeasurement(TestMeasName);
            var binName = measurement.BinaryFilePath;

            var dbm = new DbManagement(true);
            var db = dbm.GetDb(TestDbName);
            var m = db.GetMeasurement(TestMeasName);
            m.BinaryFilePath.Should().Be(binName);
        }

    }
}

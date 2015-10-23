using System;
using System.IO;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class DbManagementTests
    {
        private const string DbRootDir = @"c:\temp\DBs";
        private const string TestDbName = "test_TestDb";
        private const string TestDbName2 = "test_TestDb2";
        private IDbManagement _unitUnderTest;

        [SetUp]
        public void Setup()
        {
            _unitUnderTest = new DbManagement(true);
        }

        [TestCase]
        public void CreateDb()
        {
            _unitUnderTest.CreateDb(DbRootDir, TestDbName);

            Directory.Exists(Path.Combine(DbRootDir, TestDbName));
          
        }

        [TestCase]
        public void ListDbs()
        {
            _unitUnderTest.CreateDb(DbRootDir, TestDbName);
            _unitUnderTest.CreateDb(DbRootDir, TestDbName2);

            var dbList = _unitUnderTest.GetDbNames();

            dbList.Count.Should().BeGreaterOrEqualTo(2);

        }

        [TestCase]
        public void Deserialize()
        {
            _unitUnderTest.CreateDb(DbRootDir, TestDbName);

            var secondManagement = new DbManagement(true);
            secondManagement.GetDbNames().Count.Should().BeGreaterOrEqualTo(1);

        }
    }
}

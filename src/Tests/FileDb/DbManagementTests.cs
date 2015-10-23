using System;
using System.IO;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class DbManagementTests
    {
        private const string DbRootDir = @"c:\temp\DBs";
        private const string TestDbName = "TestDb";
        private IDbManagement _unitUnderTest;

        [SetUp]
        public void Setup()
        {
            _unitUnderTest = new DbManagement();
        }

        [TestCase]
        public void CreateDb()
        {
            _unitUnderTest.CreateDb(DbRootDir, TestDbName);

            Directory.Exists(Path.Combine(DbRootDir, TestDbName));

            
        }
    }
}

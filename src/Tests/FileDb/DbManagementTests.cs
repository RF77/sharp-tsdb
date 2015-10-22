using System;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class DbManagementTests
    {
        private IDbManagement _unitUnderTest;

        [TestInitialize]
        public void Setup()
        {
            _unitUnderTest = new DbManagement();
        }

        [TestMethod]
        public void CreateDb()
        {
            _unitUnderTest.CreateDb(@"c:\temp\DBs", "TestDb");

        }
    }
}

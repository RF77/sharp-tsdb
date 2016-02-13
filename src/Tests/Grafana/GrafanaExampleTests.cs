using System;
using System.Diagnostics;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using FluentAssertions;
using NUnit.Framework;
using QueryLanguage.Grouping;

namespace Tests.Grafana
{
    [TestFixture]
    public class GrafanaExampleTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void FixFromTo()
        {
            var dbm = new DbManagement();
            var db = dbm.GetDb("fux");

            var sw = Stopwatch.StartNew();
            var result = db.GetData<float>("Temperatur.Aussen.Wetterstation", "time > now() - 2d").GroupBy("6h", a => a.Mean());
            sw.Stop();

        }

       
    }
}
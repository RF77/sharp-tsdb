using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DbInterfaces.Interfaces;
using Deedle;
using FileDb.InterfaceImpl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using SharpTsdb.Properties;

namespace Tests
{
    [TestFixture]
    public class DeedleTests
    {
        private IDbManagement _unitUnderTest;

        [SetUp]
        public void Setup()
        {
            _unitUnderTest = new DbManagement();
        }

        [Test]
        public void CreateDb()
        {
            //_unitUnderTest.CreateDb(@"c:\temp\DBs", "TestDb");

        }

        [Test]
        public void Deedle1()
        {
            //_unitUnderTest.AttachDb("c:\\DBs\\SharpTsdb\\fux");
            var sw = Stopwatch.StartNew();
            var db = _unitUnderTest.GetDb("fux");
            
            var measurement = db.GetMeasurement("wetter");
            var dataPoints = measurement.GetDataPoints(new DateTime(2015,10,1)).ToArray();
            var b = new SeriesBuilder<DateTime, double>();
            foreach (var dataPoint in dataPoints)
            {
                b.Add(dataPoint.Key, (double) Convert.ToDouble(dataPoint.Values[0]));
            }
            Series<DateTime, double> s = b.Series;
            //var mean = s.Mean();
            //var med = s.Median();
            //var sample = s.ChunkInto()
            var resampleUniform = s.ResampleUniform(x => x.Date, x => x.AddDays(1));
            sw.Stop();
        }
    }
}

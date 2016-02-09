using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class AggregationTests
    {
        private IList<ISingleDataRow<float>> _unitUnderTest1000;
        private IList<ISingleDataRow<float>> _unitUnderTest9m;
        private List<ISingleDataRow<float>> _unitUnderTest50;

        [SetUp]
        public void Setup()
        {
            _unitUnderTest1000 = new List<ISingleDataRow<float>>();
            _unitUnderTest50 = new List<ISingleDataRow<float>>();
            int items = 1000;
            int items50 = 50;
            var startDate = new DateTime(2010, 1, 1, 13, 27, 14);
            var current40s = startDate;

            for (int i = 0; i < items; i++)
            {
                _unitUnderTest1000.Add(new SingleDataRow<float>(current40s, 0.5f * i ));
                if (i < items50)
                {
                    _unitUnderTest50.Add(new SingleDataRow<float>(current40s, 0.5f * i));
                }
                current40s += TimeSpan.FromSeconds(40);
            }
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void TestAggreggationFirst()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest50.GroupByMinutes(5, a => a.First()).ToList();
            result[0].Value.Should().Be(0);
            result[2].Value.Should().Be(6);
            result[6].Value.Should().Be(21);
            sw.Stop();
        }

        [Test]
        public void TestAggreggationHarmonicMean()
        {
            var sw = Stopwatch.StartNew();
            //var result = _unitUnderTest50.GroupByMinutes(5, a => (float)a.Values.).ToList();
            sw.Stop();
        }


    }
}
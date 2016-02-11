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
        private QueryData<float> _unitUnderTest1000;
        private QueryData<float> _unitUnderTest9m;
        private QueryData<float> _unitUnderTest50;
        private QueryData<float> _unitUnderTest50Even;
        private QueryData<int> _unitUnderTest50Int;

        [SetUp]
        public void Setup()
        {
            var unitUnderTest1000 = new List<ISingleDataRow<float>>();
            var unitUnderTest50 = new List<ISingleDataRow<float>>();
            var unitUnderTest50Int = new List<ISingleDataRow<int>>();
            var unitUnderTest50Even = new List<ISingleDataRow<float>>();
            int items = 1000;
            int items50 = 50;
            var startDate = new DateTime(2010, 1, 1, 13, 27, 14);
            var startDateEven = new DateTime(2010, 1, 1, 0, 0, 0);
            var current40s = startDate;
            var current40sEven = startDateEven;

            for (int i = 0; i < items; i++)
            {
                unitUnderTest1000.Add(new SingleDataRow<float>(current40s, 0.5f * i ));
                if (i < items50)
                {
                    unitUnderTest50.Add(new SingleDataRow<float>(current40s, 0.5f * i));
                    unitUnderTest50Int.Add(new SingleDataRow<int>(current40s, i));
                    unitUnderTest50Even.Add(new SingleDataRow<float>(current40sEven, 0.5f * i));
                }
                current40s += TimeSpan.FromSeconds(40);
                current40sEven += TimeSpan.FromSeconds(60);
            }
            _unitUnderTest1000 = new QueryData<float>(unitUnderTest1000, startDate, null);
            _unitUnderTest50 = new QueryData<float>(unitUnderTest50, startDate, null);
            _unitUnderTest50Int = new QueryData<int>(unitUnderTest50Int, startDate, null);
      
            _unitUnderTest50Even = new QueryData<float>(unitUnderTest50Even, startDateEven, null);
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void TestAggreggationFirst()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest50.GroupByMinutes(5, a => a.First()).Rows;
            result[0].Value.Should().Be(0);
            result[2].Value.Should().Be(6);
            result[6].Value.Should().Be(21);
            sw.Stop();
        }

        [Test]
        public void TestAggreggationFirstEven()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest50Even.GroupByMinutes(5, a => a.First()).Rows;
            result[0].Value.Should().Be(0);
            result[2].Value.Should().Be(5);
            result[6].Value.Should().Be(15);
            sw.Stop();
        }

        [Test]
        public void TestAggreggationLastEven()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest50Even.GroupByMinutes(5, a => a.Last()).Rows;
            result[0].Value.Should().Be(2);
            result[2].Value.Should().Be(7);
            result[6].Value.Should().Be(17);
            sw.Stop();
        }

        [Test]
        public void TestAggreggationLast()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest50.GroupByMinutes(5, a => a.Last()).Rows;
            result[0].Value.Should().Be(2);
            result[2].Value.Should().Be(9.5f);
            result[6].Value.Should().Be(24.5f);
            sw.Stop();
        }

        [Test]
        public void TestAggreggationMax()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest50.GroupByMinutes(5, a => a.Max()).Rows;
            var result2 = _unitUnderTest50.GroupByMinutes(5, a => a.Last()).Rows;
            result.Select(i => i.Value).SequenceEqual(result2.Select(i => i.Value)).Should().BeTrue();
            sw.Stop();
        }

        [Test]
        public void TestAggreggationMin()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest50Int.GroupByMinutes(5, a => a.Min()).Rows;
            var result2 = _unitUnderTest50Int.GroupByMinutes(5, a => a.First()).Rows;
            result.Select(i => i.Value).SequenceEqual(result2.Select(i => i.Value)).Should().BeTrue();
            sw.Stop();
        }


    }
}
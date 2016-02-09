using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using FluentAssertions;
using NUnit.Framework;
using QueryLanguage.Grouping;

namespace Tests.FileDb
{
    [TestFixture]
    public class GroupingTests
    {
        private IList<ISingleDataRow<float>> _unitUnderTest40s;
        private IList<ISingleDataRow<float>> _unitUnderTest9m;

        [SetUp]
        public void Setup()
        {
            _unitUnderTest40s = new List<ISingleDataRow<float>>();
            _unitUnderTest9m = new List<ISingleDataRow<float>>();
            int items = 1000;
            var startDate = new DateTime(2010, 1, 1, 13, 27, 14);
            var current40s = startDate;
            var current9m = startDate;

            for (int i = 0; i < items; i++)
            {
                _unitUnderTest40s.Add(new SingleDataRow<float>(current40s, 0.5f * i ));
                _unitUnderTest9m.Add(new SingleDataRow<float>(current9m, 0.5f * i ));
                current40s += TimeSpan.FromSeconds(40);
                current9m += TimeSpan.FromMinutes(9);
            }
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void GroupByMinutesTestStartDate()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest40s.GroupByMinutes(5, a => a.First()).ToList();
            result.First().Key.Should().Be(new DateTime(2010, 1, 1, 13, 25, 0));
            sw.Stop();
        }

        [Test]
        public void GroupByMinutes60TestStartDate()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest40s.GroupByMinutes(60, a => a.First()).ToList();
            result.First().Key.Should().Be(new DateTime(2010, 1, 1, 13, 0, 0));
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDateAndNull()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First()).ToList();
            result.First().Key.Should().Be(new DateTime(2010, 1, 1, 13, 24, 0));
            result[1].Value.Should().Be(null);
            result[10].Value.Should().Be(null);
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDateAndPrevious()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First()).Fill(ValueForNull.Previous).ToList();
            result[1].Value.Should().Be(0);
            result[10].Value.Should().Be(3);
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDateAnd5()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First()).FillValue(5).ToList();
            result[1].Value.Should().Be(5f);
            result[10].Value.Should().Be(5f);
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDateAndNext()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First()).Fill(ValueForNull.Next).ToList();
            result[1].Value.Should().Be(0.5f);
            result[10].Value.Should().Be(3.5f);
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDateAndRemoveNulls()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First()).RemoveNulls().ToList();
            result[1].Value.Should().Be(0.5f);
            result[10].Value.Should().Be(5f);
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestTimeStampEnd()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First(), TimeStampType.End).ToList();
            result[0].Key.Should().Be(new DateTime(2010, 1, 1, 13, 30, 0));
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestTimeStampMiddle()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First(), TimeStampType.Middle).ToList();
            result[0].Key.Should().Be(new DateTime(2010, 1, 1, 13, 27, 0));
            sw.Stop();
        }

    }
}
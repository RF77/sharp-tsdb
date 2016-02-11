using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using FluentAssertions;
using NUnit.Framework;
using QueryLanguage.Grouping;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class GroupingTests
    {
        private QueryData<float> _unitUnderTest40s;
        private QueryData<float> _unitUnderTest9m;
        private QueryData<float> _unitUnderTest9mNotTrimmed;
        private DateTime _trimEndDate;
        private QueryData<float> _unitUnderTest5h;

        [SetUp]
        public void Setup()
        {
            var unitUnderTest40s = new List<ISingleDataRow<float>>();
            var unitUnderTest9m = new List<ISingleDataRow<float>>();
            var unitUnderTest5h = new List<ISingleDataRow<float>>();
            var unitUnderTest9mNotTrimmed = new List<ISingleDataRow<float>>();
            //unitUnderTest9mNotTrimmed.Add(new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 0), -1f));
            //unitUnderTest9mNotTrimmed.Add(new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 1), -0.5f));
            int items = 100;
            var startDate = new DateTime(2010, 1, 1, 13, 27, 14);
            var startDate5h = new DateTime(2010, 5, 19, 13, 27, 14);
            var current40s = startDate;
            var current9m = startDate;
            var current5h = startDate5h;

            for (int i = 0; i < items; i++)
            {
                unitUnderTest40s.Add(new SingleDataRow<float>(current40s, 0.5f * i ));
                unitUnderTest9m.Add(new SingleDataRow<float>(current9m, 0.5f * i ));
                if (i < 10)
                {
                    unitUnderTest9mNotTrimmed.Add(new SingleDataRow<float>(current9m, 0.5f * i));
                }
                current40s += TimeSpan.FromSeconds(40);
                current9m += TimeSpan.FromMinutes(9);
            }

            for (int i = 0; i < 10000; i++)
            {
                unitUnderTest5h.Add(new SingleDataRow<float>(current5h, 0.5f * i));
                
                current5h += TimeSpan.FromHours(5);
            }
            //unitUnderTest9mNotTrimmed.Add(new SingleDataRow<float>(new DateTime(2010, 1, 1, 15, 0, 1), -99f));
            //unitUnderTest9mNotTrimmed.Add(new SingleDataRow<float>(new DateTime(2010, 1, 1, 15, 0, 2), -99.5f));

            _trimEndDate = new DateTime(2010, 1, 1, 15, 1, 0);
            //unitUnderTest9mNotTrimmed.Add(new SingleDataRow<float>(_trimEndDate, -99.5f));

            _unitUnderTest40s = new QueryData<float>(unitUnderTest40s, startDate, null);
            _unitUnderTest9m = new QueryData<float>(unitUnderTest9m, startDate, null);
            _unitUnderTest5h = new QueryData<float>(unitUnderTest5h, startDate5h, null);
            _unitUnderTest9mNotTrimmed = new QueryData<float>(unitUnderTest9mNotTrimmed, new DateTime(2010, 1, 1, 13, 0, 0), new DateTime(2010, 1, 1, 15, 0, 0))
            {
                PreviousRow = new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 1), -0.5f),
                NextRow = new SingleDataRow<float>(new DateTime(2010, 1, 1, 15, 0, 1), -99f)
            };
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void GroupBySeconds()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest40s.GroupBySeconds(30, a => a.First()).Rows;
            result.First().Key.Should().Be(new DateTime(2010, 1, 1, 13, 27, 0));
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDate()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest40s.GroupByMinutes(5, a => a.First()).Rows;
            result.First().Key.Should().Be(new DateTime(2010, 1, 1, 13, 25, 0));
            sw.Stop();
        }

        [Test]
        public void GroupByHours()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByHours(3, a => a.First()).Rows;
            result.First().Key.Should().Be(new DateTime(2010, 1, 1, 12, 0, 0));
            sw.Stop();
        }

        [Test]
        public void GroupByDaysAndMidnight()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.GroupByDays(3, a => a.First()).Rows;
            result.First().Key.Should().Be(new DateTime(2010, 5, 19, 0, 0, 0));
            var last = result.Last();
            sw.Stop();
        }

        [Test]
        public void GroupByDaysAnd9()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.GroupByDays(3, a => a.First(), 9).Rows;
            result[0].Key.Should().Be(new DateTime(2010, 5, 19, 9, 0, 0));
            result[1].Key.Should().Be(new DateTime(2010, 5, 22, 9, 0, 0));
            var last = result.Last();
            sw.Stop();
        }




        [Test]
        public void GroupByMonths()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.GroupByMonths(2, a => a.First()).Rows;
            result[0].Key.Should().Be(new DateTime(2010, 5, 1, 0, 0, 0));
            result[1].Key.Should().Be(new DateTime(2010, 7, 1, 0, 0, 0));
            result[10].Key.Should().Be(new DateTime(2012, 1, 1, 0, 0, 0));
            var last = result.Last();
            sw.Stop();
        }

        [Test]
        public void GroupByYears()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.GroupByYears(2, a => a.First()).Rows;
            result[0].Key.Should().Be(new DateTime(2010, 1, 1, 0, 0, 0));
            result[1].Key.Should().Be(new DateTime(2012, 1, 1, 0, 0, 0));
            result[2].Key.Should().Be(new DateTime(2014, 1, 1, 0, 0, 0));
            var last = result.Last();
            sw.Stop();
        }

        [Test]
        public void GroupByMinutes60TestStartDate()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest40s.GroupByMinutes(60, a => a.First()).Rows;
            result.First().Key.Should().Be(new DateTime(2010, 1, 1, 13, 0, 0));
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDateAndNull()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First()).Rows;
            result.First().Key.Should().Be(new DateTime(2010, 1, 1, 13, 24, 0));
            result[1].Value.Should().Be(null);
            result[10].Value.Should().Be(null);
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDateAndNullNotTrimmed()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9mNotTrimmed.GroupByMinutes(6, a => a.First()).Rows;
            result.First().Key.Should().Be(new DateTime(2010, 1, 1, 13, 0, 0));
            result.Last().Key.Should().Be(new DateTime(2010, 1, 1, 14, 54, 0));
            result[1].Value.Should().Be(null);
            result.Last().Value.Should().Be(null);
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDateAndPrevious()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First()).Fill(ValueForNull.Previous).Rows;
            result[1].Value.Should().Be(0);
            result[10].Value.Should().Be(3);
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDateAnd5()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First()).FillValue(5).Rows;
            result[1].Value.Should().Be(5f);
            result[10].Value.Should().Be(5f);
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDateAndNext()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First()).Fill(ValueForNull.Next).Rows;
            result[1].Value.Should().Be(0.5f);
            result[10].Value.Should().Be(3.5f);
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDateAndRemoveNulls()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First()).RemoveNulls().Rows;
            result[1].Value.Should().Be(0.5f);
            result[10].Value.Should().Be(5f);
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestTimeStampEnd()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First(), TimeStampType.End).Rows;
            result[0].Key.Should().Be(new DateTime(2010, 1, 1, 13, 30, 0));
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestTimeStampMiddle()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First(), TimeStampType.Middle).Rows;
            result[0].Key.Should().Be(new DateTime(2010, 1, 1, 13, 27, 0));
            sw.Stop();
        }

    }
}
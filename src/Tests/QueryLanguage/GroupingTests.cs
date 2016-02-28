using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using FluentAssertions;
using NUnit.Framework;
using Timeenator.Impl;
using Timeenator.Impl.Grouping;
using Timeenator.Interfaces;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class GroupingTests
    {
        private QuerySerie<float> _unitUnderTest40s;
        private QuerySerie<float> _unitUnderTest9m;
        private QuerySerie<float> _unitUnderTest9mNotTrimmed;
        private DateTime _trimEndDate;
        private QuerySerie<float> _unitUnderTest5h;

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

            _unitUnderTest40s = new QuerySerie<float>(unitUnderTest40s, startDate, null);
            _unitUnderTest9m = new QuerySerie<float>(unitUnderTest9m, startDate, null);
            _unitUnderTest5h = new QuerySerie<float>(unitUnderTest5h, startDate5h, null);
            _unitUnderTest9mNotTrimmed = new QuerySerie<float>(unitUnderTest9mNotTrimmed, new DateTime(2010, 1, 1, 13, 0, 0), new DateTime(2010, 1, 1, 15, 0, 0))
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
            var result2 = _unitUnderTest40s.Group(c => c.ByTime.Seconds(30).Aggregate(a => a.First())).Rows;
            result.First().TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 27, 0));
            result.SequenceEqual(result2).Should().BeTrue();
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDate()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest40s.GroupByMinutes(5, a => a.First()).Rows;
            var result2 = _unitUnderTest40s.Group(c=>c.ByTime.Minutes(5).Aggregate(a => a.First())).Rows;
            result.First().TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 25, 0));
            result.SequenceEqual(result2).Should().BeTrue();
            sw.Stop();
        }

        [Test]
        public void GroupByHours()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByHours(3, a => a.First()).Rows;
            var result2 = _unitUnderTest9m.Group(c => c.ByTime.Hours(3).Aggregate(a => a.First())).Rows;
            result.SequenceEqual(result2).Should().BeTrue();
            result.First().TimeUtc.Should().Be(new DateTime(2010, 1, 1, 12, 0, 0));
            sw.Stop();
        }

        [Test]
        public void GroupByDaysAndMidnight()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.GroupByDays(3, a => a.First()).Rows;
            var result2 = _unitUnderTest5h.Group(g => g.ByTime.Days(3).Aggregate(a => a.First())).Rows;
            result.First().TimeUtc.Should().Be(new DateTime(2010, 5, 19, 0, 0, 0));
            result.SequenceEqual(result2).Should().BeTrue();
            var last = result.Last();
            sw.Stop();
        }

        [Test]
        public void GroupByDaysAnd9()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.GroupByDays(3, a => a.First(), 9).Rows;
            var result2 = _unitUnderTest5h.Group(c => c.ByTime.Days(3, 9).Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 5, 19, 9, 0, 0));
            result[1].TimeUtc.Should().Be(new DateTime(2010, 5, 22, 9, 0, 0));
            result.SequenceEqual(result2).Should().BeTrue();
            var last = result.Last();
            sw.Stop();
        }




        [Test]
        public void GroupByMonths()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.GroupByMonths(2, a => a.First()).Rows;
            var result2 = _unitUnderTest5h.Group(g => g.ByTime.Months(2).Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 5, 1, 0, 0, 0));
            result[1].TimeUtc.Should().Be(new DateTime(2010, 7, 1, 0, 0, 0));
            result[10].TimeUtc.Should().Be(new DateTime(2012, 1, 1, 0, 0, 0));
            result.SequenceEqual(result2).Should().BeTrue();
            var last = result.Last();
            sw.Stop();
        }


        [Test]
        public void GroupByWeeks()
        {
            var dayOfWeek = DayOfWeek.Monday;
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.GroupByWeeks(1, a => a.First()).Rows;
            var result2 = _unitUnderTest5h.Group(g => g.ByTime.Weeks(1).Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 5, 17, 0, 0, 0));
            result[0].TimeUtc.DayOfWeek.Should().Be(dayOfWeek);
            result[1].TimeUtc.Should().Be(new DateTime(2010, 5, 24, 0, 0, 0));
            result[1].TimeUtc.DayOfWeek.Should().Be(dayOfWeek);
            result[13].TimeUtc.DayOfWeek.Should().Be(dayOfWeek);
            result.Last().TimeUtc.DayOfWeek.Should().Be(dayOfWeek);
            result.SequenceEqual(result2).Should().BeTrue();

            sw.Stop();
        }

        [Test]
        public void GroupByWeeksStartingSaturday()
        {
            var dayOfWeek = DayOfWeek.Saturday;
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.GroupByWeeks(1, a => a.First(), dayOfWeek).Rows;
            var result2 = _unitUnderTest5h.Group(g => g.ByTime.Weeks(1, dayOfWeek).Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 5, 15, 0, 0, 0));
            result[0].TimeUtc.DayOfWeek.Should().Be(dayOfWeek);
            result[1].TimeUtc.Should().Be(new DateTime(2010, 5, 22, 0, 0, 0));
            result[1].TimeUtc.DayOfWeek.Should().Be(dayOfWeek);
            result[13].TimeUtc.DayOfWeek.Should().Be(dayOfWeek);
            result.Last().TimeUtc.DayOfWeek.Should().Be(dayOfWeek);
            result.SequenceEqual(result2).Should().BeTrue();

            sw.Stop();
        }

        [Test]
        public void GroupByYears()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.GroupByYears(2, a => a.First()).Rows;
            var result2 = _unitUnderTest5h.Group(g => g.ByTime.Years(2).Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 0, 0, 0));
            result[1].TimeUtc.Should().Be(new DateTime(2012, 1, 1, 0, 0, 0));
            result[2].TimeUtc.Should().Be(new DateTime(2014, 1, 1, 0, 0, 0));
            var last = result.Last();
            result.SequenceEqual(result2).Should().BeTrue();
            sw.Stop();
        }

        [Test]
        public void GroupByMinutes60TestStartDate()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest40s.GroupByMinutes(60, a => a.First()).Rows;
            var result2 = _unitUnderTest40s.Group(g => g.ByTime.Minutes(60).Aggregate(a => a.First())).Rows;
            result.First().TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 0, 0));
            result.SequenceEqual(result2).Should().BeTrue();
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDateAndNull()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First()).Rows;
            var result2 = _unitUnderTest9m.Group(g => g.ByTime.Minutes(6).Aggregate(a => a.First())).Rows;
            result.First().TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 24, 0));
            result[1].Value.Should().Be(null);
            result[10].Value.Should().Be(null);
            sw.Stop();
            result.SequenceEqual(result2).Should().BeTrue();
        }

        [Test]
        public void GroupByMinutesTestStartDateAndNullNotTrimmed()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9mNotTrimmed.GroupByMinutes(6, a => a.First()).Rows;
            var result2 = _unitUnderTest9mNotTrimmed.Group(c => c.ByTime.Minutes(6).Aggregate(a => a.First())).Rows;
            result.First().TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 0, 0));
            result.Last().TimeUtc.Should().Be(new DateTime(2010, 1, 1, 14, 54, 0));
            result[1].Value.Should().Be(null);
            result.Last().Value.Should().Be(null);
            result.SequenceEqual(result2).Should().BeTrue();
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDateAndPrevious()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First()).Fill(ValueForNull.Previous).Rows;
            var result2 = _unitUnderTest9m.Group(g => g.ByTime.Minutes(6).Aggregate(a => a.First())).Fill(ValueForNull.Previous).Rows;
            result[1].Value.Should().Be(0);
            result[10].Value.Should().Be(3);
            result.SequenceEqual(result2).Should().BeTrue();
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDateAnd5()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First()).FillValue(5).Rows;
            var result2 = _unitUnderTest9m.Group(g => g.ByTime.Minutes(6).Aggregate(a => a.First())).FillValue(5).Rows;
            result[1].Value.Should().Be(5f);
            result[10].Value.Should().Be(5f);
            result.SequenceEqual(result2).Should().BeTrue();
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
            var result2 = _unitUnderTest9m.Group(g => g.ByTime.Minutes(6).TimeStampIsEnd().Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 30, 0));
            result.SequenceEqual(result2).Should().BeTrue();
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestTimeStampMiddle()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First(), TimeStampType.Middle).Rows;
            var result2 = _unitUnderTest9m.Group(g => g.ByTime.Minutes(6).TimeStampIsMiddle().Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 27, 0));
            result.SequenceEqual(result2).Should().BeTrue();
            sw.Stop();
        }


        [Test]
        public void GroupByExpressionSeconds()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest40s.GroupBy("30s", a => a.First(), null, TimeStampType.Middle).Rows;
            var result2 = _unitUnderTest40s.Group(g => g.ByTime.Expression("30s").TimeStampIsMiddle().Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 27, 15));
            result[1].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 27, 45));
            result.SequenceEqual(result2).Should().BeTrue();
            sw.Stop();
        }

        [Test]
        public void GroupByExpressionMinutes()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest40s.GroupBy("6m", a => a.First(), null, TimeStampType.Middle).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 27, 0));
            result[1].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 33, 0));
            sw.Stop();
        }

        [Test]
        public void GroupByExpressionHours()
        {
            var sw = Stopwatch.StartNew();
            INullableQuerySerie<float> nullableQuerySerie = _unitUnderTest9m.GroupBy("3h", a => a.First(), null, TimeStampType.Start);
            IReadOnlyList<ISingleDataRow<float?>> result = nullableQuerySerie.Rows;
            IObjectQuerySerie result2 = nullableQuerySerie;
            object val = result2.Rows.First().Value;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 12, 0, 0));
            result[1].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 15, 0, 0));
            sw.Stop();
        }

        [Test]
        public void GroupByExpressionHoursButExpanded()
        {
            var sw = Stopwatch.StartNew();
            INullableQuerySerie<float> nullableQuerySerie = _unitUnderTest9m.GroupBy("3h", a => a.First(), null, TimeStampType.Start);
            IReadOnlyList<ISingleDataRow<float?>> result = nullableQuerySerie.Rows;
            IObjectQuerySerie result2 = nullableQuerySerie;
            object val = result2.Rows.First().Value;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 12, 0, 0));
            result[1].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 15, 0, 0));
            sw.Stop();
        }

    }
}
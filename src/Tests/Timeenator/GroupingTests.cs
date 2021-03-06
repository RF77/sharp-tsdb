// /*******************************************************************************
//  * Copyright (c) 2016 by RF77 (https://github.com/RF77)
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the Eclipse Public License v1.0
//  * which accompanies this distribution, and is available at
//  * http://www.eclipse.org/legal/epl-v10.html
//  *
//  * Contributors:
//  *    RF77 - initial API and implementation and/or initial documentation
//  *******************************************************************************/ 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Timeenator.Extensions.Grouping;
using Timeenator.Impl;
using Timeenator.Impl.Grouping;
using Timeenator.Interfaces;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class GroupingTests
    {
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
            var startDate = new DateTime(2010, 1, 1, 13, 27, 14, DateTimeKind.Utc);
            var startDate5h = new DateTime(2010, 5, 19, 13, 27, 14, DateTimeKind.Utc);
            var current40s = startDate;
            var current9m = startDate;
            var current5h = startDate5h;

            for (int i = 0; i < items; i++)
            {
                unitUnderTest40s.Add(new SingleDataRow<float>(current40s, 0.5f*i));
                unitUnderTest9m.Add(new SingleDataRow<float>(current9m, 0.5f*i));
                if (i < 10)
                {
                    unitUnderTest9mNotTrimmed.Add(new SingleDataRow<float>(current9m, 0.5f*i));
                }
                current40s += TimeSpan.FromSeconds(40);
                current9m += TimeSpan.FromMinutes(9);
            }

            for (int i = 0; i < 10000; i++)
            {
                unitUnderTest5h.Add(new SingleDataRow<float>(current5h, 0.5f*i));

                current5h += TimeSpan.FromHours(5);
            }
            //unitUnderTest9mNotTrimmed.Add(new SingleDataRow<float>(new DateTime(2010, 1, 1, 15, 0, 1), -99f));
            //unitUnderTest9mNotTrimmed.Add(new SingleDataRow<float>(new DateTime(2010, 1, 1, 15, 0, 2), -99.5f));

            _trimEndDate = new DateTime(2010, 1, 1, 15, 1, 0, DateTimeKind.Utc);
            //unitUnderTest9mNotTrimmed.Add(new SingleDataRow<float>(_trimEndDate, -99.5f));

            _unitUnderTest40s = new QuerySerie<float>(unitUnderTest40s, startDate, null);
            _unitUnderTest9m = new QuerySerie<float>(unitUnderTest9m, startDate, null);
            _unitUnderTest5h = new QuerySerie<float>(unitUnderTest5h, startDate5h, null);
            _unitUnderTest9mNotTrimmed = new QuerySerie<float>(unitUnderTest9mNotTrimmed,
                new DateTime(2010, 1, 1, 13, 0, 0, DateTimeKind.Utc),
                new DateTime(2010, 1, 1, 15, 0, 0, DateTimeKind.Utc))
            {
                PreviousRow = new SingleDataRow<float>(new DateTime(2010, 1, 1, 0, 0, 1, DateTimeKind.Utc), -0.5f),
                NextRow = new SingleDataRow<float>(new DateTime(2010, 1, 1, 15, 0, 1, DateTimeKind.Utc), -99f)
            };
        }

        [TearDown]
        public void TearDown()
        {
        }

        private QuerySerie<float> _unitUnderTest40s;
        private QuerySerie<float> _unitUnderTest9m;
        private QuerySerie<float> _unitUnderTest9mNotTrimmed;
        private DateTime _trimEndDate;
        private QuerySerie<float> _unitUnderTest5h;

        [Test]
        public void GroupByDaysAnd9()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.Group(g => g.ByTime.Days(3, 9).Aggregate(a => a.First())).Rows;
            var result2 = _unitUnderTest5h.Group(c => c.ByTime.Days(3, 9).Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 5, 19, 9, 0, 0, DateTimeKind.Local).ToUniversalTime());
            result[1].TimeUtc.Should().Be(new DateTime(2010, 5, 22, 9, 0, 0, DateTimeKind.Local).ToUniversalTime());
            result.SequenceEqual(result2).Should().BeTrue();
            var last = result.Last();
            sw.Stop();
        }

        [Test]
        public void GroupByDaysAndMidnight()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.GroupByDays(3, a => a.First()).Rows;
            var result2 = _unitUnderTest5h.Group(g => g.ByTime.Days(3).Aggregate(a => a.First())).Rows;
            result.First().TimeUtc.Should().Be(new DateTime(2010, 5, 19, 0, 0, 0, DateTimeKind.Local).ToUniversalTime());
            result.SequenceEqual(result2).Should().BeTrue();
            var last = result.Last();
            sw.Stop();
        }

        [Test]
        public void GroupByExpressionHours()
        {
            var sw = Stopwatch.StartNew();
            INullableQuerySerie<float> nullableQuerySerie = _unitUnderTest9m.GroupBy("3h", a => a.First());
            IReadOnlyList<ISingleDataRow<float?>> result = nullableQuerySerie.Rows;
            IObjectQuerySerie result2 = nullableQuerySerie;
            object val = result2.Rows.First().Value;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 12, 0, 0, DateTimeKind.Utc));
            result[1].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 15, 0, 0, DateTimeKind.Utc));
            sw.Stop();
        }

        [Test]
        public void GroupByExpressionHoursButExpanded()
        {
            var sw = Stopwatch.StartNew();
            INullableQuerySerie<float> nullableQuerySerie = _unitUnderTest9m.GroupBy("3h", a => a.First());
            IReadOnlyList<ISingleDataRow<float?>> result = nullableQuerySerie.Rows;
            IObjectQuerySerie result2 = nullableQuerySerie;
            object val = result2.Rows.First().Value;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 12, 0, 0, DateTimeKind.Utc));
            result[1].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 15, 0, 0, DateTimeKind.Utc));
            sw.Stop();
        }

        [Test]
        public void GroupByExpressionMinutes()
        {
            var sw = Stopwatch.StartNew();
            var result =
                _unitUnderTest40s.Group(g => g.ByTime.Expression("6m").TimeStampIsMiddle().Aggregate(a => a.First()))
                    .Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 27, 0, DateTimeKind.Utc));
            result[1].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 33, 0, DateTimeKind.Utc));
            sw.Stop();
        }

        [Test]
        public void GroupByExpressionSeconds()
        {
            var sw = Stopwatch.StartNew();
            var result =
                _unitUnderTest40s.Group(g => g.ByTime.Expression("30s").TimeStampIsMiddle().Aggregate(a => a.First()))
                    .Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 27, 15, DateTimeKind.Utc));
            result[1].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 27, 45, DateTimeKind.Utc));
            sw.Stop();
        }

        [Test]
        public void GroupByHours()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByHours(3, a => a.First()).Rows;
            var result2 = _unitUnderTest9m.Group(c => c.ByTime.Hours(3).Aggregate(a => a.First())).Rows;
            result.SequenceEqual(result2).Should().BeTrue();
            result.First().TimeUtc.Should().Be(new DateTime(2010, 1, 1, 12, 0, 0, DateTimeKind.Utc));
            sw.Stop();
        }

        [Test]
        public void GroupByMinutes60TestStartDate()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest40s.GroupByMinutes(60, a => a.First()).Rows;
            var result2 = _unitUnderTest40s.Group(g => g.ByTime.Minutes(60).Aggregate(a => a.First())).Rows;
            result.First().TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 0, 0, DateTimeKind.Utc));
            result.SequenceEqual(result2).Should().BeTrue();
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestStartDate()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest40s.GroupByMinutes(5, a => a.First()).Rows;
            var result2 = _unitUnderTest40s.Group(c => c.ByTime.Minutes(5).Aggregate(a => a.First())).Rows;
            result.First().TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 25, 0, DateTimeKind.Utc));
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
        public void GroupByMinutesTestStartDateAndNull()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest9m.GroupByMinutes(6, a => a.First()).Rows;
            var result2 = _unitUnderTest9m.Group(g => g.ByTime.Minutes(6).Aggregate(a => a.First())).Rows;
            result.First().TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 24, 0, DateTimeKind.Utc));
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
            result.First().TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 0, 0, DateTimeKind.Utc));
            result.Last().TimeUtc.Should().Be(new DateTime(2010, 1, 1, 14, 54, 0, DateTimeKind.Utc));
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
            var result2 =
                _unitUnderTest9m.Group(g => g.ByTime.Minutes(6).Aggregate(a => a.First()))
                    .Fill(ValueForNull.Previous)
                    .Rows;
            result[1].Value.Should().Be(0);
            result[10].Value.Should().Be(3);
            result.SequenceEqual(result2).Should().BeTrue();
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
            var result =
                _unitUnderTest9m.Group(g => g.ByTime.Minutes(6).TimeStampIsEnd().Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 30, 0, DateTimeKind.Utc));
            sw.Stop();
        }

        [Test]
        public void GroupByMinutesTestTimeStampMiddle()
        {
            var sw = Stopwatch.StartNew();
            var result =
                _unitUnderTest9m.Group(g => g.ByTime.Minutes(6).TimeStampIsMiddle().Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 27, 0, DateTimeKind.Utc));
            sw.Stop();
        }

        [Test]
        public void GroupByMonths()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.Group(g => g.ByTime.Months(2).Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 5, 1, 0, 0, 0, DateTimeKind.Local).ToUniversalTime());
            result[1].TimeUtc.Should().Be(new DateTime(2010, 7, 1, 0, 0, 0, DateTimeKind.Local).ToUniversalTime());
            result[10].TimeUtc.Should().Be(new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Local).ToUniversalTime());
            var last = result.Last();
            sw.Stop();
        }

        [Test]
        public void GroupBySeconds()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest40s.GroupBySeconds(30, a => a.First()).Rows;
            var result2 = _unitUnderTest40s.Group(c => c.ByTime.Seconds(30).Aggregate(a => a.First())).Rows;
            result.First().TimeUtc.Should().Be(new DateTime(2010, 1, 1, 13, 27, 0, DateTimeKind.Utc));
            result.SequenceEqual(result2).Should().BeTrue();
            sw.Stop();
        }

        [Test]
        public void GroupByWeeks()
        {
            var dayOfWeek = DayOfWeek.Monday;
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.Group(g => g.ByTime.Weeks(1).Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 5, 17, 0, 0, 0, DateTimeKind.Local).ToUniversalTime());
            result[0].TimeUtc.ToLocalTime().DayOfWeek.Should().Be(dayOfWeek);
            result[1].TimeUtc.Should().Be(new DateTime(2010, 5, 24, 0, 0, 0, DateTimeKind.Local).ToUniversalTime());
            result[1].TimeUtc.ToLocalTime().DayOfWeek.Should().Be(dayOfWeek);
            result[13].TimeUtc.ToLocalTime().DayOfWeek.Should().Be(dayOfWeek);
            var dateTime = result.Last().TimeUtc;
            var localTime = dateTime.ToLocalTime();
            //weeks is not daylight save
            //localTime.DayOfWeek.Should().Be(dayOfWeek);

            sw.Stop();
        }

        [Test]
        public void GroupByWeeksStartingSaturday()
        {
            var dayOfWeek = DayOfWeek.Saturday;
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.Group(g => g.ByTime.Weeks(1, dayOfWeek).Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 5, 15, 0, 0, 0, DateTimeKind.Local).ToUniversalTime());
            result[0].TimeUtc.ToLocalTime().DayOfWeek.Should().Be(dayOfWeek);
            result[1].TimeUtc.Should().Be(new DateTime(2010, 5, 22, 0, 0, 0, DateTimeKind.Local).ToUniversalTime());
            result[1].TimeUtc.ToLocalTime().DayOfWeek.Should().Be(dayOfWeek);
            result[13].TimeUtc.ToLocalTime().DayOfWeek.Should().Be(dayOfWeek);
            //result.Last().TimeUtc.DayOfWeek.Should().Be(dayOfWeek);

            sw.Stop();
        }

        [Test]
        public void GroupByYears()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest5h.Group(g => g.ByTime.Years(2).Aggregate(a => a.First())).Rows;
            result[0].TimeUtc.Should().Be(new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Local).ToUniversalTime());
            result[1].TimeUtc.Should().Be(new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Local).ToUniversalTime());
            result[2].TimeUtc.Should().Be(new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Local).ToUniversalTime());
            var last = result.Last();
            sw.Stop();
        }
    }
}
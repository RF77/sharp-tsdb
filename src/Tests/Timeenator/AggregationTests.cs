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
using Timeenator.Interfaces;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class AggregationTests
    {
        [SetUp]
        public void Setup()
        {
            var unitUnderTest50 = new List<ISingleDataRow<float>>();
            var unitUnderTest50Int = new List<ISingleDataRow<int>>();
            var unitUnderTest50Even = new List<ISingleDataRow<float>>();
            int items = 1000;
            int items50 = 50;
            var startDate = new DateTime(2010, 1, 1, 13, 27, 14, DateTimeKind.Utc);
            var startDateEven = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var current40S = startDate;
            var current40SEven = startDateEven;

            for (int i = 0; i < items; i++)
            {
                //unitUnderTest1000.Add(new SingleDataRow<float>(current40s, 0.5f * i ));
                if (i < items50)
                {
                    unitUnderTest50.Add(new SingleDataRow<float>(current40S, 0.5f*i));
                    unitUnderTest50Int.Add(new SingleDataRow<int>(current40S, i));
                    unitUnderTest50Even.Add(new SingleDataRow<float>(current40SEven, 0.5f*i));
                }
                current40S += TimeSpan.FromSeconds(40);
                current40SEven += TimeSpan.FromSeconds(60);
            }
            //_unitUnderTest1000 = new QuerySerie<float>(unitUnderTest1000, startDate, null);
            _unitUnderTest50 = new QuerySerie<float>(unitUnderTest50, startDate, null);
            _unitUnderTest50Int = new QuerySerie<int>(unitUnderTest50Int, startDate, null);

            _unitUnderTest50Even = new QuerySerie<float>(unitUnderTest50Even, startDateEven, null);
        }

        [TearDown]
        public void TearDown()
        {
        }

        private QuerySerie<float> _unitUnderTest50;
        private QuerySerie<float> _unitUnderTest50Even;
        private QuerySerie<int> _unitUnderTest50Int;

        [Test]
        public void ExpMeanTest()
        {
            var sw = Stopwatch.StartNew();
            //var result = _unitUnderTest50.GroupBySeconds(5, a => a.MeanByTime()).Rows;
            //var result2 = _unitUnderTest50.GroupBySeconds(5, a => a.MeanByTimeIncludePrevious()).RemoveNulls().Rows;
            var result3 =
                _unitUnderTest50.Group(
                    g =>
                        g.ByTime.Seconds(5)
                            .TimeStampIsMiddle()
                            .ExpandTimeRangeByFactor(50)
                            .Aggregate(a => a.MeanExpWeighted())).Rows;

            sw.Stop();
        }

        [Test]
        public void MeanByTimeIncludePreviousTest()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest50.GroupBySeconds(5, a => a.MeanByTime()).Rows;
            var result2 =
                _unitUnderTest50.GroupBySeconds(5, a => a.MeanByTimeIncludePreviousAndNext()).RemoveNulls().Rows;

            result.Count.Should().Be(result2.Count);

            sw.Stop();
        }

        [Test]
        public void TestAggreggationDifference()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest50Int.GroupByMinutes(5, a => a.Difference()).Rows;
            sw.Stop();
            (_unitUnderTest50Int.Rows.Last().Value - _unitUnderTest50Int.Rows.First().Value)
                .Should().Be(result.Sum(i => i.Value));
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
        public void TestAggreggationMax()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest50.GroupByMinutes(5, a => a.Max()).Rows;
            var result2 = _unitUnderTest50.GroupByMinutes(5, a => a.Last()).Rows;
            result.Select(i => i.Value).SequenceEqual(result2.Select(i => i.Value)).Should().BeTrue();
            sw.Stop();
        }

        [Test]
        public void TestAggreggationMedian()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest50Int.GroupByMinutes(5, a => a.Median()).Rows;
            sw.Stop();
            result[6].Value.Should().Be(46);
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

        [Test]
        public void TestAggreggationSum()
        {
            var sw = Stopwatch.StartNew();
            var result = _unitUnderTest50Int.GroupByMinutes(5, a => a.Sum()).Rows;
            sw.Stop();
            result[6].Value.Should().Be(364);
        }
    }
}
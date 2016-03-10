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
using FluentAssertions;
using NUnit.Framework;
using Timeenator.Impl;
using Timeenator.Impl.Grouping;
using Timeenator.Interfaces;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class WhereGroupingTests
    {
        [SetUp]
        public void Setup()
        {
            _rows.Clear();
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 0, 0), 2));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 1, 0, DateTimeKind.Utc), 4));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 8, 0, DateTimeKind.Utc), 6));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 11, 0, DateTimeKind.Utc), 8));
        }

        [TearDown]
        public void TearDown()
        {
        }

        private readonly List<ISingleDataRow<float>> _rows = new List<ISingleDataRow<float>>();

        private QuerySerie<float> PrepareForGroupTests()
        {
            var serie = new QuerySerie<float>(_rows, new DateTime(1000, 1, 1),
                new DateTime(1000, 1, 1, 0, 15, 0, DateTimeKind.Utc))
            {
                PreviousRow = new SingleDataRow<float>(new DateTime(99, 1, 1, 0, 11, 0, DateTimeKind.Utc), -3),
                NextRow = new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 16, 0, DateTimeKind.Utc), 11)
            };
            return serie;
        }

        [Test]
        public void GroupByGroupTimes1()
        {
            var serie = PrepareForGroupTests();
            StartEndTime groupTime = new StartEndTime(new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(1000, 1, 1, 0, 10, 0, DateTimeKind.Utc));
            ;

            //var result = serie.GroupBy(new[] { groupTime }, i => i.MeanByTime());
            var result = serie.Group(g => g.ByTimeRanges(new[] {groupTime}).Aggregate(i => i.MeanByTime()));
            result.Rows.Count.Should().Be(1);
            result.Rows[0].TimeUtc.Should().Be(new DateTime(1000, 1, 1, 0, 0, 0));
            result.Rows[0].Value.Should().Be(4.2f);
        }

        [Test]
        public void GroupByGroupTimesStartBeforeStartOfSerie()
        {
            var serie = PrepareForGroupTests();
            var startTime = new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc) - TimeSpan.FromMinutes(1);
            StartEndTime groupTime = new StartEndTime(startTime, new DateTime(1000, 1, 1, 0, 10, 0, DateTimeKind.Utc));
            ;

            var result = serie.Group(g => g.ByTimeRanges(new[] {groupTime}).Aggregate(i => i.MeanByTime()));
            result.Rows.Count.Should().Be(1);
            result.Rows[0].TimeUtc.Should().Be(startTime);
            result.Rows[0].Value.Should().BeLessThan(4.2f);
        }

        [Test]
        public void GroupByGroupTimesStartWithPrevAndNext()
        {
            var serie = PrepareForGroupTests();
            var startTime = new DateTime(1000, 1, 1, 0, 2, 0, DateTimeKind.Utc);
            StartEndTime groupTime = new StartEndTime(startTime, new DateTime(1000, 1, 1, 0, 9, 0, DateTimeKind.Utc));
            ;

            var result = serie.Group(g => g.ByTimeRanges(new[] {groupTime}).Aggregate(i => i.MeanByTime()));
            result.Rows.Count.Should().Be(1);
            result.Rows[0].TimeUtc.Should().Be(startTime);
            result.Rows[0].Value.Should().BeApproximately(4.285f, 3);
        }

        [Test]
        public void Test()
        {
        }
    }
}
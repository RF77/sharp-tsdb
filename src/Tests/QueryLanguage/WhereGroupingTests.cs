using System;
using System.Collections.Generic;
using DbInterfaces.Interfaces;
using FluentAssertions;
using NUnit.Framework;
using Timeenator.Extensions.Grouping;
using Timeenator.Impl;
using Timeenator.Impl.Grouping;
using Timeenator.Interfaces;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class WhereGroupingTests
    {
        private List<ISingleDataRow<float>> _rows = new List<ISingleDataRow<float>>();

        [SetUp]
        public void Setup()
        {
            _rows.Clear();
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1), 2));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 1, 0), 4));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 8, 0), 6));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 11, 0), 8));
        }


        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void GroupByGroupTimes1()
        {
            var serie = PrepareForGroupTests();
            StartEndTime groupTime = new StartEndTime(new DateTime(1000, 1, 1, 0, 0, 0), new DateTime(1000, 1, 1, 0, 10, 0)); ;

            //var result = serie.GroupBy(new[] { groupTime }, i => i.MeanByTime());
            var result = serie.Group(g => g.ByTimeRanges(new[] { groupTime }).Aggregate(i => i.MeanByTime()));
            result.Rows.Count.Should().Be(1);
            result.Rows[0].TimeUtc.Should().Be(new DateTime(1000, 1, 1));
            result.Rows[0].Value.Should().Be(4.2f);
        }

        [Test]
        public void GroupByGroupTimesStartBeforeStartOfSerie()
        {
            var serie = PrepareForGroupTests();
            var startTime = new DateTime(1000, 1, 1, 0, 0, 0) - TimeSpan.FromMinutes(1);
            StartEndTime groupTime = new StartEndTime(startTime, new DateTime(1000, 1, 1, 0, 10, 0)); ;

            var result = serie.Group(g => g.ByTimeRanges(new[] { groupTime }).Aggregate(i => i.MeanByTime()));
            result.Rows.Count.Should().Be(1);
            result.Rows[0].TimeUtc.Should().Be(startTime);
            result.Rows[0].Value.Should().BeLessThan(4.2f);
        }

        [Test]
        public void GroupByGroupTimesStartWithPrevAndNext()
        {
            var serie = PrepareForGroupTests();
            var startTime = new DateTime(1000, 1, 1, 0, 2, 0);
            StartEndTime groupTime = new StartEndTime(startTime, new DateTime(1000, 1, 1, 0, 9, 0)); ;

            var result = serie.Group(g => g.ByTimeRanges(new[] { groupTime }).Aggregate(i => i.MeanByTime()));
            result.Rows.Count.Should().Be(1);
            result.Rows[0].TimeUtc.Should().Be(startTime);
            result.Rows[0].Value.Should().BeApproximately(4.285f, 3);
        }

        private QuerySerie<float> PrepareForGroupTests()
        {
            var serie = new QuerySerie<float>(_rows, new DateTime(1000, 1, 1), new DateTime(1000, 1, 1, 0, 15, 0))
            {
                PreviousRow = new SingleDataRow<float>(new DateTime(99, 1, 1, 0, 11, 0), -3),
                NextRow = new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 16, 0), 11)
            };
            return serie;
        }


        [Test]
        public void Test()
        {
        }


    }
}
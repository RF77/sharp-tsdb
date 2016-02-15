using System;
using System.Collections.Generic;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using FluentAssertions;
using NUnit.Framework;
using QueryLanguage.Grouping;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class MeanByTimeTests
    {
        private List<ISingleDataRow<float>> _rows = new List<ISingleDataRow<float>>();

        [SetUp]
        public void Setup()
        {
            _rows.Clear();
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1), 2));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 1, 0), 4));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 8, 0), 6));
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void WithoutPrevAndNext()
        {
            var serie = new QuerySerie<float>(_rows, new DateTime(1000,1,1), new DateTime(1000,1,1,0,10,0));
            var result = serie.MeanByTime();
            result.Should().Be(3.75f);
        }

        [Test]
        public void WithoutPrev()
        {
            var serie = new QuerySerie<float>(_rows, new DateTime(999, 1, 1), new DateTime(1000, 1, 1, 0, 10, 0))
            {
                NextRow = new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 11, 0), 11)
            };
            var result = serie.MeanByTime();
            result.Should().Be(4.2f);
        }

        [Test]
        public void FirstItemOnStartTime_PrevRowHasNoImpact()
        {
            var serie = new QuerySerie<float>(_rows, new DateTime(1000, 1, 1), new DateTime(1000, 1, 1, 0, 10, 0))
            {
                PreviousRow = new SingleDataRow<float>(new DateTime(99, 1, 1, 0, 11, 0), 10),
                NextRow = new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 11, 0), 11)
            };
            var result = serie.MeanByTime();
            result.Should().Be(4.2f);
        }

        [Test]
        public void WithPrevAndNext()
        {
            var serie = new QuerySerie<float>(_rows, new DateTime(1000, 1, 1)-TimeSpan.FromMinutes(5), new DateTime(1000, 1, 1, 0, 10, 0))
            {
                PreviousRow = new SingleDataRow<float>(new DateTime(999, 1, 1, 0, 11, 0), 9.6f),
                NextRow = new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 11, 0), 11)
            };
            var result = serie.MeanByTime();
            result.Should().Be(6f);
        }


    }
}
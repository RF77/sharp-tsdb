using System;
using System.Collections.Generic;
using DbInterfaces.Interfaces;
using FluentAssertions;
using NUnit.Framework;
using Timeenator.Extensions.Converting;
using Timeenator.Extensions.Grouping;
using Timeenator.Impl;
using Timeenator.Impl.Grouping;
using Timeenator.Interfaces;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class QuerySerieTests
    {
        private List<ISingleDataRow<float>> _rows = new List<ISingleDataRow<float>>();

        [SetUp]
        public void Setup()
        {
            _rows.Clear();
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1,0,0,0, DateTimeKind.Utc), 2));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 1, 0, DateTimeKind.Utc), 4));
            _rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 8, 0, DateTimeKind.Utc), 6));
        }


        [TearDown]
        public void TearDown()
        {
        }



        [Test]
        public void IncludeLastRowTest()
        {
            var serie = new QuerySerie<float>(_rows, new DateTime(1000, 1, 1,0,0,0, DateTimeKind.Utc), new DateTime(1000, 1, 1, 0, 10, 0, DateTimeKind.Utc));

            var result = serie.IncludeLastRow();
            result.IncludeLastRow().Should().NotBeNull();
        }

        [Test]
        public void ZipSeriesTest()
        {
            var serie = new QuerySerie<float>(_rows, new DateTime(1000, 1, 1,0,0,0, DateTimeKind.Utc), new DateTime(1000, 1, 1, 0, 10, 0, DateTimeKind.Utc)).GroupByMinutes(5, i => i.First());
            var serie2 = new QuerySerie<float>(_rows, new DateTime(1000, 1, 1,0,0,0, DateTimeKind.Utc), new DateTime(1000, 1, 1, 0, 10, 0, DateTimeKind.Utc)).GroupByMinutes(5, i => i.Last()); ;

            var result = serie.Zip(serie2, "res", (s1, s2) => s1 + s2);
            result.Name.Should().Be("res");
            result.Rows[0].Value.Should().Be(6);
            result.Rows[1].Value.Should().Be(12);
        }


        [Test]
        public void WhereTest()
        {
            IQuerySerie<float> serie = new QuerySerie<float>(_rows, new DateTime(1000, 1, 1,0,0,0, DateTimeKind.Utc), new DateTime(1000, 1, 1, 0, 10, 0, DateTimeKind.Utc));
            var newSerie = serie.Where(i => i.TimeUtc > new DateTime(1000, 1, 1, 0, 5, 0, DateTimeKind.Utc));
            serie.Rows.Count.Should().Be(3);
            newSerie.Rows.Count.Should().Be(1);

            var newSerie2 = serie.Where(i => i.Value >= 4);
            serie.Rows.Count.Should().Be(3);
            newSerie2.Rows.Count.Should().Be(2);
        }

        [Test]
        public void WhereValueTest()
        {
            IQuerySerie<float> serie = new QuerySerie<float>(_rows, new DateTime(1000, 1, 1,0,0,0, DateTimeKind.Utc), new DateTime(1000, 1, 1, 0, 10, 0, DateTimeKind.Utc));
            var newSerie2 = serie.WhereValue(i => i >= 4);
            serie.Rows.Count.Should().Be(3);
            newSerie2.Rows.Count.Should().Be(2);
        }

        [Test]
        public void SawToothDefault()
        {
            var rows = new List<ISingleDataRow<float>>();
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1,0,0,0, DateTimeKind.Utc), 2));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 1, 0, DateTimeKind.Utc), 4));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 8, 0, DateTimeKind.Utc), 6));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 10, 0, DateTimeKind.Utc), 4));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 11, 0, DateTimeKind.Utc), 6));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 12, 0, DateTimeKind.Utc), 4));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 13, 0, DateTimeKind.Utc), 6));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 14, 0, DateTimeKind.Utc), 4));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 15, 0, DateTimeKind.Utc), 6));

            var serie = new QuerySerie<float>(rows, new DateTime(1000, 1, 1,0,0,0, DateTimeKind.Utc), new DateTime(1000, 1, 1, 0, 15, 0, DateTimeKind.Utc));

            var normalizedSerie = serie.NormalizeOverflows();
            normalizedSerie.Rows[8].Value.Should().Be(24);

        }

        [Test]
        public void SawToothWithResetValueOne()
        {
            var rows = new List<ISingleDataRow<float>>();
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1,0,0,0, DateTimeKind.Utc), 2));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 1, 0, DateTimeKind.Utc), 4));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 8, 0, DateTimeKind.Utc), 6));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 10, 0, DateTimeKind.Utc), 4));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 11, 0, DateTimeKind.Utc), 6));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 12, 0, DateTimeKind.Utc), 4));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 13, 0, DateTimeKind.Utc), 6));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 14, 0, DateTimeKind.Utc), 4));
            rows.Add(new SingleDataRow<float>(new DateTime(1000, 1, 1, 0, 15, 0, DateTimeKind.Utc), 6));

            var serie = new QuerySerie<float>(rows, new DateTime(1000, 1, 1,0,0,0, DateTimeKind.Utc), new DateTime(1000, 1, 1, 0, 15, 0, DateTimeKind.Utc));

            var normalizedSerie = serie.NormalizeOverflows(1);
            normalizedSerie.Rows[8].Value.Should().Be(15);

        }


        [Test]
        public void Test()
        {
        }

    }
}
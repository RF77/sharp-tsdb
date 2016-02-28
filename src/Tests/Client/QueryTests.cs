using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy.Json;
using NUnit.Framework;
using SharpTsdbClient;
using Timeenator.Extensions.Grouping;
using Timeenator.Impl;
using Timeenator.Impl.Grouping;

namespace Tests.Client
{
    [TestFixture, Ignore("Service has to run... Manual tests..")]
    public class QueryTests
    {
        private DbClient _dbClient;

        [SetUp]
        public void Setup()
        {
            _dbClient = new SharpTsdbClient.Client("localhost").Db("fux");
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task Query1Test()
        {
            var result =
                await _dbClient.QuerySerieAsync<float>(
                    db => db.GetSerie<float>("Innen.EG.Wohnzimmer.Temperatur", "time > now() - 1M")
                        .Group(g => g.ByTime.Days(1).Aggregate(i => i.Mean())));
        }

        [Test]
        public async Task WritePoints1Test()
        {
            var measName = "test";
            await _dbClient.Measurement(measName).ClearMeasurementAsync();
            var empty = await _dbClient.QuerySerieAsync<long>(db => db.GetSerie<long>(measName));

            var dateTime = new DateTime(2010, 1, 1, 0, 0, 0);
            var data = new[]
            {
                new SingleDataRow<long>(dateTime, 5),
                new SingleDataRow<long>(new DateTime(2010, 1, 1, 0, 0, 20), 77),
                new SingleDataRow<long>(DateTime.Now, 1977),
            };
            await _dbClient.Measurement(measName).AppendAsync(data, true);
            var queryResult = await _dbClient.QuerySerieAsync<long>(db => db.GetSerie<long>(measName));
            queryResult.Rows[0].TimeUtc.Should().Be(dateTime);
            queryResult.Rows.Count.Should().Be(3);
            await _dbClient.Measurement(measName).ClearMeasurementAsync(new DateTime(2010, 1, 1, 0, 0, 1));
            var queryResult2 = await _dbClient.QuerySerieAsync<long>(db => db.GetSerie<long>(measName));
            queryResult2.Rows.Count.Should().Be(1);
            await _dbClient.Measurement(measName).ClearMeasurementAsync();
        }

        [Test]
        public void TypeToStringTest()
        {
            var type = typeof (float);
            var name = type.Name;

        }


    }
}
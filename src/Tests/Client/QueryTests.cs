using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy.Json;
using NUnit.Framework;
using SharpTsdbClient;
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


    }
}
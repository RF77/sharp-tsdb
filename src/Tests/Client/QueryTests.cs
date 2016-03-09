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
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SharpTsdbClient;
using Timeenator.Extensions.Grouping;
using Timeenator.Impl;

namespace Tests.Client
{
    [TestFixture, Ignore("Service has to run... Manual tests..")]
    public class QueryTests
    {
        [SetUp]
        public void Setup()
        {
            _dbClient = new SharpTsdbClient.Client("10.10.1.77").Db("fux");
        }

        [TearDown]
        public void TearDown()
        {
        }

        private DbClient _dbClient;

        [Test]
        public async Task Helper()
        {
            //var db = new SharpTsdbClient.Client("localhost").Db("Haus");
            var db = new SharpTsdbClient.Client("localhost").Db("fux");

            var result =
                await
                    db.QueryTableAsync<float>(
                        d =>
                            d.GetTable<float>("(?<g>.*).(?<k>Temperatur).*", "time > now() - 1M")
                                .Transform(s => s.GroupBy("1d", a => a.Mean())));
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
        public void TypeToStringTest()
        {
            var type = typeof (float);
            var name = type.Name;
        }

        [Test]
        public async Task WritePoints1Test()
        {
            var measName = "test";
            await _dbClient.Measurement(measName).ClearAsync();
            var empty = await _dbClient.QuerySerieAsync<long>(db => db.GetSerie<long>(measName));

            var dateTime = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var data = new[]
            {
                new SingleDataRow<long>(dateTime, 5),
                new SingleDataRow<long>(new DateTime(2010, 1, 1, 0, 0, 20, DateTimeKind.Utc), 77),
                new SingleDataRow<long>(DateTime.Now, 1977)
            };
            await _dbClient.Measurement(measName).AppendAsync(data, true);
            var queryResult = await _dbClient.QuerySerieAsync<long>(db => db.GetSerie<long>(measName));
            queryResult.Rows[0].TimeUtc.Should().Be(dateTime);
            queryResult.Rows.Count.Should().Be(3);
            await
                _dbClient.Measurement(measName)
                    .ClearAsync(new DateTime(2010, 1, 1, 0, 0, 1, DateTimeKind.Utc));
            var queryResult2 = await _dbClient.QuerySerieAsync<long>(db => db.GetSerie<long>(measName));
            queryResult2.Rows.Count.Should().Be(1);
            await _dbClient.Measurement(measName).ClearAsync();
        }

 
    }
}
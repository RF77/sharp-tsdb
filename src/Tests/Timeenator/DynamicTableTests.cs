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
using FileDb.Impl;
using FileDb.Scripting;
using FluentAssertions;
using NUnit.Framework;
using Timeenator.Extensions.Converting;
using Timeenator.Extensions.Grouping;
using Timeenator.Extensions.Scientific;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class DynamicTableTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        private int DummyLambda(Func<dynamic, dynamic> testFunc)
        {
            return (int) testFunc(5);
        }

        private dynamic Dummy(dynamic a)
        {
            return a + 2;
        }

        [Test]
        public void DynamicSerieTests()
        {
            DummyLambda(a => a);
            Dummy(3);

            const int max = 100000;
            var start = new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var rowsA = new List<ISingleDataRow<int>>();
            var rowsB = new List<ISingleDataRow<int>>();


            for (var i = 0; i < max; i++)
            {
                rowsA.Add(new SingleDataRow<int>(start + TimeSpan.FromMinutes(i), i));
                rowsB.Add(new SingleDataRow<int>(start + TimeSpan.FromMinutes(i), i*2));
            }

            var serieA = new QuerySerie<int>(rowsA, start, start + TimeSpan.FromMinutes(max)) {Name = "A", Key = "A"};
            var serieB = new QuerySerie<int>(rowsB, start, start + TimeSpan.FromMinutes(max)) {Name = "B", Key = "B"};
            var table = new QueryTable<int>();
            table.AddSerie(serieA);
            table.AddSerie(serieB);

            //var sw = Stopwatch.StartNew();
            //var result = table.Do(i => i.GroupByHours(1, t => t.Mean())).ZipToNew<int>("SumTable", t => t.A + t.B);
            //sw.Stop();

            var sw2 = Stopwatch.StartNew();
            var result2 = table.Transform(i => i.GroupByMinutes(1, t => t.First()))
                .ZipToNew("DiffTable", t => t.A - t.B);
            sw2.Stop();
        }

        [Test]
        public void DynamicTableInScript()
        {
            var db = new DbManagement().GetDb("fux");
            var result =
                new ScriptingEngine(db,
                    @"db.GetTable<float>(""Aussen.Wetterstation.(?<k>[TF]).*?$"", ""time > now() - 1M"")
.Transform(i => i.GroupByHours(1, o => o.Mean()))
.ZipAndAdd(""Sum"", t => t.T + t.F)").Execute();
        }

        [Test]
        public void DynamicWithRealDb()
        {
            var db = new DbManagement().GetDb("fux");
            var queryTable = db.GetTable<float>("Aussen.Wetterstation.(?<k>[TF]).*?$", "time > now() - 1M");
            var result = queryTable
                .Group(g => g.ByTime.Hours(1).Aggregate(o => o.Mean()))
                .Calc(t => t.Sum = t.T + t.F);

            result.Series.Count().Should().Be(3);

            result = result.RemoveDbSeries();

            result.Series.Count().Should().Be(1);

        }

        [Test]
        public void MultipleAbsoluteHumidityTableGroups()
        {
            var db = new DbManagement().GetDb("fux");
            var result =
                db.GetTable<float>("Innen.(?<g>.*).(?<k>Temperatur|Feuchtigkeit)$", "time > now() - 1M")
                    .Transform(g => g.Group(i => i.ByTime.Hours(1).ExpandTimeRangeByFactor(7).Aggregate(o => o.Mean())))
                    .GroupSeries().Transform(t => t.AbsoluteHumidity("Temperatur", "Feuchtigkeit")).MergeTables();
        }

        [Test]
        public void MultipleTableGroups()
        {
            var db = new DbManagement().GetDb("fux");
            var queryTable = db.GetTable<float>("Innen.(?<g>.*).(?<k>Temperatur|Feuchtigkeit)$", "time > now() - 1M");
            var result = queryTable
                .Transform(i => i.GroupByHours(1, o => o.Mean()))
                .ZipToNew("Sum", t => t.Temperatur + t.Feuchtigkeit);
        }
    }
}
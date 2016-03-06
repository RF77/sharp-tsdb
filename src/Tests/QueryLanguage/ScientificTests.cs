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

using CustomDbExtensions;
using FileDb.Impl;
using NUnit.Framework;
using Timeenator.Extensions.Grouping;
using Timeenator.Extensions.Scientific;

namespace Tests.QueryLanguage
{
    [TestFixture]
    public class ScientificTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void AllCalcTempValuesTest()
        {
            var db = new DbManagement().GetDb("fux");
            var queryTable = db.GetTable<float>("Aussen.Wetterstation.(?<k>.*?)$", "time > now() - 1M");
            var result = queryTable
                .Transform(i => i.GroupByHours(1, o => o.Mean()))
                .AddDewPoint("Temperatur", "Feuchtigkeit")
                .AddAbsoluteHumidity("Temperatur", "Feuchtigkeit")
                .AddHumidex("Temperatur", "Feuchtigkeit")
                .AddSleetLine("Temperatur", "Feuchtigkeit", 440)
                .AddSnowLine("Temperatur", "Feuchtigkeit", 440)
                .RemoveSerie("Feuchtigkeit")
                ;
        }

        [Test]
        public void SchneefallTest()
        {
            var db = new DbManagement().GetDb("Haus");
            var result = db.Schneefallgrenze("time > now() - 1d", "1m", 50);
        }

        [Test]
        public void DewPoinTest()
        {
            var db = new DbManagement().GetDb("Haus");
            var result = db.DewPoint("Hm.*(?<g>(EG|OG).*)(?<k>Temp|Hum)","time > now() - 1d", "1m", "Temp", "Hum", 50);
        }

        [Test]
        public void AbsHumTest()
        {
            var db = new DbManagement().GetDb("Haus");
            var result = db.AbsHumidity("Hm.*(?<g>(EG|OG).*)(?<k>Temp|Hum)", "time > now() - 1d", "1m", "Temp", "Hum", 50);
        }



        [Test]
        public void DewPointTest()
        {
            var db = new DbManagement().GetDb("fux");
            var queryTable = db.GetTable<float>("Aussen.Wetterstation.(?<k>.*?)$", "time > now() - 1y");
            var result = queryTable
                .Transform(i => i.GroupByHours(1, o => o.Mean()))
                .DewPoint("Temperatur", "Feuchtigkeit", "Taupunkt");
        }

        [Test]
        public void MovingAverage()
        {
            var db = new DbManagement().GetDb("fux");
            var time = "time < now() - 14d and time > now() - 21d";

            var serie = db.GetSerie<float>("Aussen.Wetterstation.Temperatur", time)
                .Group(
                    g =>
                        g.ByTime.Minutes(10)
                            .TimeStampIsMiddle()
                            .ExpandTimeRangeByFactor(20)
                            .Aggregate(a => a.MeanByTime()));
        }
    }
}
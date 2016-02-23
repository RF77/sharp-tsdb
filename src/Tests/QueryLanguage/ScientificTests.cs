using System.Linq;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;
using NUnit.Framework;
using Timeenator.Impl;
using Timeenator.Impl.Converting;
using Timeenator.Impl.Grouping;
using Timeenator.Impl.Scientific;
using Timeenator.Interfaces;

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
        public void DewPointTest()
        {
            var db = new DbManagement().GetDb("fux");
            var queryTable = db.GetTable<float>("Aussen.Wetterstation.(?<k>.*?)$", "time > now() - 1y");
            var result = queryTable
                .Do(i => TimeGroupingExtensions.GroupByHours<float>(i, 1, o => AggregationExtensions.Mean<float>(o)))
                .DewPoint("Temperatur", "Feuchtigkeit", "Taupunkt");
        }

        [Test]
        public void AllCalcTempValuesTest()
        {
            var db = new DbManagement().GetDb("fux");
            var queryTable = db.GetTable<float>("Aussen.Wetterstation.(?<k>.*?)$", "time > now() - 1M");
            var result = queryTable
                .Do(i => i.GroupByHours(1, o => o.Mean()))
                .AddDewPoint("Temperatur", "Feuchtigkeit")
                .AddAbsoluteHumidity("Temperatur", "Feuchtigkeit")
                .AddHumidex("Temperatur", "Feuchtigkeit")
                .AddSleetLine("Temperatur", "Feuchtigkeit", 440)
                .AddSnowLine("Temperatur", "Feuchtigkeit", 440)
                .RemoveSerie("Feuchtigkeit")
                ;
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
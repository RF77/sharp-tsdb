using System;
using DbInterfaces.Interfaces;
using Timeenator.Extensions.Converting;
using Timeenator.Extensions.Scientific;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace CustomDbExtensions
{
    public static class LueftungsExtensions
    {
        //public static INullableQueryTable<float> Lueftungswerte(this IDb db, string time, string interval, int windowFactor)
        //{
        //    var table = new QueryTable<float>();
        //    table.AddSerie(db.GetSerie<float>("Lueftung_Zulufttemperatur", time).Alias("Zuluft"));
        //    table.AddSerie(db.GetSerie<float>("Lueftung_Ablufttemperatur", time).Alias("Abluft"));
        //    table.AddSerie(db.GetSerie<float>("Lueftung_Aussenlufttemperatur", time).Alias("Aussenluft"));
        //    table.AddSerie(db.GetSerie<float>("Lueftung_Fortlufttemperatur", time).Alias("Fortluft"));
        //    table.AddSerie(db.GetSerie<float>("HmWetterstationTemperature", time).Alias("AussenTemp"));
        //    table.AddSerie(db.GetSerie<float>("heizenAvgTemperatureOhneHobbyraum", time).Alias("InnenTemp"));

        //    table.Transform(
        //            i =>
        //                i.Group(
        //                    g =>
        //                        g.ByTime.Expression(interval, interval, windowFactor)
        //                            .ExpandTimeRange(TimeSpan.FromMinutes(windowFactor)).TimeStampIsMiddle()
        //                            .Aggregate(a => a.MeanExpWeighted())))
        //                            .GroupSeries().Transform(t => t.AbsoluteHumidity(temperatureKey, humidityKey, null)).MergeTables();
        //}
    }
}
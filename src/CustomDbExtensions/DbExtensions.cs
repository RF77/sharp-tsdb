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
using DbInterfaces.Interfaces;
using Timeenator.Extensions.Converting;
using Timeenator.Extensions.Scientific;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace CustomDbExtensions
{
    public static class DbExtensions
    {
        public static NullableQueryTable<float> AverageTest(this IDb db)
        {

            var time = "time < now() - 14d and time > now() - 21d";
            var table = new NullableQueryTable<float>();
            table.AddSerie(db.GetSerie<float>("Aussen.Wetterstation.Temperatur", time).Alias("roh").ToNullable());
            table.AddSerie(
                db.GetSerie<float>("Aussen.Wetterstation.Temperatur", time)
                    .Group(g => g.ByTime.Minutes(10).TimeStampIsMiddle().Aggregate(a => a.Mean()))
                    .Alias("mean"));
            table.AddSerie(
                db.GetSerie<float>("Aussen.Wetterstation.Temperatur", time)
                    .Group(g => g.ByTime.Minutes(10).TimeStampIsMiddle().Aggregate(a => a.MeanByTime()))
                    .Alias("mean by time"));
            table.AddSerie(
                db.GetSerie<float>("Aussen.Wetterstation.Temperatur", time)
                    .Group(
                        g =>
                            g.ByTime.Minutes(10)
                                .TimeStampIsMiddle()
                                .ExpandTimeRangeByFactor(3)
                                .Aggregate(a => a.MeanByTime()))
                    .Alias("mov mean 3"));
            table.AddSerie(
                db.GetSerie<float>("Aussen.Wetterstation.Temperatur", time)
                    .Group(
                        g =>
                            g.ByTime.Minutes(10)
                                .TimeStampIsMiddle()
                                .ExpandTimeRangeByFactor(7)
                                .Aggregate(a => a.MeanByTime()))
                    .Alias("mov mean 7"));
            table.AddSerie(
                db.GetSerie<float>("Aussen.Wetterstation.Temperatur", time)
                    .Group(
                        g =>
                            g.ByTime.Minutes(10)
                                .TimeStampIsMiddle()
                                .ExpandTimeRangeByFactor(12)
                                .Aggregate(a => a.MeanByTime()))
                    .Alias("mov mean 12"));

            table.AddSerie(
                db.GetSerie<float>("Aussen.Wetterstation.Temperatur", time)
                    .Group(
                        g =>
                            g.ByTime.Minutes(10)
                                .TimeStampIsMiddle()
                                .ExpandTimeRangeByFactor(20)
                                .Aggregate(a => a.MeanByTime()))
                    .Alias("mov mean 20"));
            table.AddSerie(
                db.GetSerie<float>("Aussen.Wetterstation.Temperatur", time)
                    .Group(
                        g =>
                            g.ByTime.Minutes(10)
                                .TimeStampIsMiddle()
                                .ExpandTimeRangeByFactor(50)
                                .Aggregate(a => a.MeanByTime()))
                    .Alias("mov mean 50"));


            return table;
        }

        public static NullableQueryTable<float> MovingAverage(this IDb db, string measurementName, string time,
            string interval)
        {
            var table = new NullableQueryTable<float>();
            table.AddSerie(db.GetSerie<float>(measurementName, time).Alias("roh").ToNullable());
            table.AddSerie(
                db.GetSerie<float>(measurementName, time)
                    .Group(g => g.ByTime.Expression(interval, "3m").TimeStampIsMiddle().Aggregate(a => a.Mean()))
                    .Alias("mean"));
            table.AddSerie(
                db.GetSerie<float>(measurementName, time)
                    .Group(g => g.ByTime.Expression(interval, "3m").TimeStampIsMiddle().Aggregate(a => a.MeanByTime()))
                    .Alias("mean by time"));
            table.AddSerie(
                db.GetSerie<float>(measurementName, time)
                    .Group(
                        g =>
                            g.ByTime.Expression(interval, "3m")
                                .TimeStampIsMiddle()
                                .ExpandTimeRangeByFactor(3)
                                .Aggregate(a => a.MeanByTime()))
                    .Alias("mov mean 3"));
            table.AddSerie(
                db.GetSerie<float>(measurementName, time)
                    .Group(
                        g =>
                            g.ByTime.Expression(interval, "3m")
                                .TimeStampIsMiddle()
                                .ExpandTimeRangeByFactor(5)
                                .Aggregate(a => a.MeanByTime()))
                    .Alias("mov mean 5"));
            table.AddSerie(
                db.GetSerie<float>(measurementName, time)
                    .Group(
                        g =>
                            g.ByTime.Expression(interval, "3m")
                                .TimeStampIsMiddle()
                                .ExpandTimeRangeByFactor(7)
                                .Aggregate(a => a.MeanByTime()))
                    .Alias("mov mean 7"));
            var movMean12 = db.GetSerie<float>(measurementName, time)
                .Group(
                    g =>
                        g.ByTime.Expression(interval, "3m")
                            .TimeStampIsMiddle()
                            .ExpandTimeRangeByFactor(12)
                            .Aggregate(a => a.MeanByTime()));
            table.AddSerie(
                movMean12
                    .Alias("mov mean 12"));

            var movMean20 = db.GetSerie<float>(measurementName, time)
                .Group(
                    g =>
                        g.ByTime.Expression(interval, "3m")
                            .TimeStampIsMiddle()
                            .ExpandTimeRangeByFactor(20)
                            .Aggregate(a => a.MeanByTime()));
            table.AddSerie(
                movMean20
                    .Alias("mov mean 20"));
            table.AddSerie(
                db.GetSerie<float>(measurementName, time)
                    .Group(
                        g =>
                            g.ByTime.Expression(interval, "3m")
                                .TimeStampIsMiddle()
                                .ExpandTimeRangeByFactor(50)
                                .Aggregate(a => a.MeanByTime()))
                    .Alias("mov mean 50"));
            table.AddSerie(movMean12.RemoveNulls()
                .Group(
                    g =>
                        g.ByTime.Expression(interval, "3m")
                            .ExpandTimeRangeByFactor(3)
                            .Aggregate(a => a.Derivative(TimeSpan.FromHours(1))))
                .Alias("Diff 1h 12"));
            table.AddSerie(movMean12.RemoveNulls()
                .Group(
                    g =>
                        g.ByTime.Expression(interval, "3m")
                            .ExpandTimeRangeByFactor(3)
                            .Aggregate(a => a.Derivative(TimeSpan.FromDays(1))))
                .Alias("Diff 1d 12"));
            table.AddSerie(movMean20.RemoveNulls()
                .Group(
                    g =>
                        g.ByTime.Expression(interval, "3m")
                            .ExpandTimeRangeByFactor(3)
                            .Aggregate(a => a.Derivative(TimeSpan.FromHours(1))))
                .Alias("Diff 1h 20"));
            table.AddSerie(movMean20.RemoveNulls()
                .Group(
                    g =>
                        g.ByTime.Expression(interval, "3m")
                            .ExpandTimeRangeByFactor(3)
                            .Aggregate(a => a.Derivative(TimeSpan.FromDays(1))))
                .Alias("Diff 1d 20"));
            table.AddSerie(movMean20.RemoveNulls()
                .Group(
                    g =>
                        g.ByTime.Expression(interval, "3m")
                            .ExpandTimeRangeByFactor(12)
                            .Aggregate(a => a.Derivative(TimeSpan.FromHours(1))))
                .Alias("Diff 1h 20 12"));
            table.AddSerie(movMean20.RemoveNulls()
                .Group(
                    g =>
                        g.ByTime.Expression(interval, "3m")
                            .ExpandTimeRangeByFactor(12)
                            .Aggregate(a => a.Derivative(TimeSpan.FromDays(1))))
                .Alias("Diff 1d 20 12"));

            return table;
        }

        public static INullableQueryTable<float> MovingAverage(this IDb db, string measurementName, string time,
            string interval, int windowFactor)
        {
            return db.GetTable<float>(measurementName, time)
                .Transform(
                    i =>
                        i.Group(
                            g =>
                                g.ByTime.Expression(interval, "1m")
                                    .ExpandTimeRangeByFactor(windowFactor).TimeStampIsMiddle()
                                    .Aggregate(a => a.MeanByTime())));
        }

        public static INullableQueryTable<float> ExpMovingAverage(this IDb db, string measurementName, string time,
            string interval, int windowFactor)
        {
            return db.GetTable<float>(measurementName, time)
                .Transform(
                    i =>
                        i.Group(
                            g =>
                                g.ByTime.Expression(interval, "1m", 10)
                                    .ExpandTimeRange(TimeSpan.FromMinutes(windowFactor)).TimeStampIsMiddle()
                                    .Aggregate(a => a.MeanExpWeighted())));
        }

        public static INullableQueryTable<float> Schneefallgrenze(this IDb db, string time,
           string interval, int windowFactor)
        {
            return db.GetTable<float>("(?<g>Wetterstation).*(?<k>Temp|Hum)", time)
                .Transform(
                    i =>
                        i.Group(
                            g =>
                                g.ByTime.Expression(interval, "1m", 10)
                                    .ExpandTimeRange(TimeSpan.FromMinutes(windowFactor)).TimeStampIsMiddle()
                                    .Aggregate(a => a.MeanExpWeighted()))).AddSleetLine("Temp","Hum",440,"Schneeregengrenze").AddSnowLine("Temp", "Hum", 440, "Schneefallgrenze")
                                    .RemoveSerie("Temp").RemoveSerie("Hum");
        }

        public static INullableQueryTable<float> DewPoint(this IDb db, string measurementName, string time,
          string interval, string temperatureKey, string humidityKey, int windowFactor)
        {
            return db.GetTable<float>(measurementName, time)
                .Transform(
                    i =>
                        i.Group(
                            g =>
                                g.ByTime.Expression(interval, "1m", 10)
                                    .ExpandTimeRange(TimeSpan.FromMinutes(windowFactor)).TimeStampIsMiddle()
                                    .Aggregate(a => a.MeanExpWeighted())))
                                    .GroupSeries().Transform(t => t.DewPoint(temperatureKey, humidityKey, null)).MergeTables();
        }

        public static INullableQueryTable<float> Heat(this IDb db, string measurementName, string time,
          string interval, string temperatureKey, string humidityKey, int windowFactor)
        {
            var table = db.GetTable<float>(measurementName, time)
                .Transform(i => i.Group(g =>g.ByTime.Expression(interval, "1m", 10).ExpandTimeRange(TimeSpan.FromMinutes(windowFactor)).TimeStampIsMiddle()
                                    .Aggregate(a => a.MeanExpWeighted())))
                                    .GroupSeries()
                                    .Transform(t => t.AddHeatIndex(temperatureKey, humidityKey))
                                    .Transform(t => t.AddHumidex(temperatureKey, humidityKey))
                                    .MergeTables();

            return table;
        }

        public static INullableQueryTable<float> AbsHumidity(this IDb db, string measurementName, string time,
          string interval, string temperatureKey, string humidityKey, int windowFactor)
        {
            return db.GetTable<float>(measurementName, time)
                .Transform(
                    i =>
                        i.Group(
                            g =>
                                g.ByTime.Expression(interval, "1m", 10)
                                    .ExpandTimeRange(TimeSpan.FromMinutes(windowFactor)).TimeStampIsMiddle()
                                    .Aggregate(a => a.MeanExpWeighted())))
                                    .GroupSeries().Transform(t => t.AbsoluteHumidity(temperatureKey, humidityKey, null)).MergeTables();
        }

        public static INullableQueryTable<float> MovingTest(this IDb db, string measurementName, string time,
            string interval, int windowFactor)
        {
            return db.GetTable<float>(measurementName, time)
                .Transform(
                    i =>
                        i.Group(
                            g =>
                                g.ByTime.Expression(interval, "1m")
                                    .ExpandTimeRange(TimeSpan.FromMinutes(windowFactor)).TimeStampIsMiddle()
                                    .Aggregate(a => a.MeanExpWeighted())).AppendName(".Exp")).MergeTable(
                                        db.GetTable<float>(measurementName, time)
                                            .Transform(
                                                i =>
                                                    i.Group(
                                                        g =>
                                                            g.ByTime.Expression(interval, "1m")
                                                                .ExpandTimeRangeByFactor(windowFactor)
                                                                .TimeStampIsMiddle()
                                                                .Aggregate(a => a.MeanByTime())).AppendName(".Mean"))
                ).MergeTable(
                    db.GetTable<float>(measurementName, time)
                        .Transform(
                            i =>
                                i.Group(
                                    g =>
                                        g.ByTime.Expression(interval, "1m")
                                            .TimeStampIsMiddle()
                                            .Aggregate(a => a.MeanByTime())))
                );
        }
    }
}
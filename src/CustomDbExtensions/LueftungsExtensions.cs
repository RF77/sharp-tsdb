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
        private static float KwlElectricalPower(float fanLevel)
        {
            if (fanLevel < 1.1)
            {
                return 13;
            }
            if (fanLevel < 2.1)
            {
                return 26;
            }
            if (fanLevel < 3.1)
            {
                return 80;
            }
            if (fanLevel < 4.1)
            {
                return 160;
            }
            return 350;
        }

        private static float AirFlow(float fanLevel)
        {
            if (fanLevel < 1.1)
            {
                return 45;
            }
            if (fanLevel < 2.1)
            {
                return 120;
            }
            if (fanLevel < 3.1)
            {
                return 200;
            }
            if (fanLevel < 4.1)
            {
                return 300;
            }
            return 400;
        }

        private const float Cp = 0.324f;

        private static float AirPower(float fanLevel, float? tA, float? tB)
        {
            if (tA == null || tB == null) return 0;
            return AirFlow(fanLevel) * Cp * (tA.Value - tB.Value);
        }

        public static INullableQueryTable<float> LueftungTemperaturen(this IDb db, string time, string interval,
            int windowFactor)
        {
            var table = new QueryTable<float>();
            table.AddSerie(db.GetSerie<float>("Lueftung_Zulufttemperatur", time).Alias("Zulufttemperatur"));
            table.AddSerie(db.GetSerie<float>("Lueftung_Ablufttemperatur", time).Alias("Ablufttemperatur"));
            table.AddSerie(db.GetSerie<float>("Lueftung_Aussenlufttemperatur", time).Alias("Aussenlufttemperatur"));
            table.AddSerie(db.GetSerie<float>("Lueftung_Fortlufttemperatur", time).Alias("Fortlufttemperatur"));
            table.AddSerie(db.GetSerie<float>("Lueftung_Fan_Level", time).Alias("Lüfterstufe"));
            table.AddSerie(db.GetSerie<float>("HmWetterstationTemperature", time).Alias("Aussentemperatur Wetterstation"));
            table.AddSerie(db.GetSerie<float>("heizenAvgTemperatureOhneHobbyraum", time).Alias("Innenraumtemperatur"));
            return table.Group(
                g =>
                    g.ByTime.Expression(interval, "5m", windowFactor)
                        .ExpandTimeRange(TimeSpan.FromMinutes(windowFactor)).TimeStampIsMiddle()
                        .Aggregate(a => a.MeanExpWeighted()));
        }

        public static INullableQueryTable<float> Lueftungswerte(this IDb db, string time, string interval, int windowFactor)
        {
            var table = new QueryTable<float>();
            table.AddSerie(db.GetSerie<float>("Lueftung_Zulufttemperatur", time).Alias("Zuluft"));
            table.AddSerie(db.GetSerie<float>("Lueftung_Ablufttemperatur", time).Alias("Abluft"));
            table.AddSerie(db.GetSerie<float>("Lueftung_Aussenlufttemperatur", time).Alias("Aussenluft"));
            table.AddSerie(db.GetSerie<float>("Lueftung_Fortlufttemperatur", time).Alias("Fortluft"));
            table.AddSerie(db.GetSerie<float>("Lueftung_Fan_Level", time).Alias("FanLevel"));
            table.AddSerie(db.GetSerie<float>("HmWetterstationTemperature", time).Alias("AussenTemp"));
            table.AddSerie(db.GetSerie<float>("heizenAvgTemperatureOhneHobbyraum", time).Alias("InnenTemp"));

            return table.Group(
                g =>
                    g.ByTime.Expression(interval, "5m", windowFactor)
                        .ExpandTimeRange(TimeSpan.FromMinutes(windowFactor)).TimeStampIsMiddle()
                        .Aggregate(a => a.MeanExpWeighted()))
                .Calc(t =>
                {
                    t.SoleEntzugsleistung = AirPower(t.FanLevel, t.Aussenluft, t.AussenTemp);
                    t.WirkungsgradKwlAbluft = (float) 100.0/(t.Abluft - t.Aussenluft)*(t.Abluft - t.Fortluft);
                    t.WirkungsgradGesamt = (float)100.0 / (t.InnenTemp - t.AussenTemp) * (t.Zuluft - t.AussenTemp);
                    t.WirkungsgradOhneLeitungsverlust = (float)100.0 / (t.Abluft - t.AussenTemp) * (t.Zuluft - t.AussenTemp);
                    t.WirkungsgradKwlZuluft = (float)100.0 / (t.Abluft - t.Aussenluft) * (t.Zuluft - t.Aussenluft);
                    t.GewinnDurchSole = (float)t.SoleEntzugsleistung * ((100.0f - t.WirkungsgradKwlAbluft) / 100.0);
                    var arbeitsZahl = 4.5f;
                    t.GewinnDurchSoleElektrisch = (float)t.GewinnDurchSole / arbeitsZahl;
                    t.GewinnNettoDurchSoleElektrisch = (float)(t.GewinnDurchSole / arbeitsZahl) - 2f;
                    t.VerlustLeitungen = (float)AirPower(t.FanLevel, t.InnenTemp, t.Abluft);
                    t.VerlustHaus = AirPower(t.FanLevel, t.InnenTemp, t.Zuluft);
                    t.GewinnKwl = AirPower(t.FanLevel, t.Zuluft, t.Aussenluft);
                    t.VerlustKwlElektrisch = KwlElectricalPower(t.FanLevel);
                    t.GewinnNettoElektrisch = (t.GewinnKwl + t.GewinnDurchSole) / arbeitsZahl - KwlElectricalPower(t.FanLevel) - 2;
                }).RemoveDbSeries();
        }
    }
}
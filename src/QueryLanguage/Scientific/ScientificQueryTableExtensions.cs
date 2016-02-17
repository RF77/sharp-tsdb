using System;
using DbInterfaces.Interfaces;
using QueryLanguage.Converting;
using QueryLanguage.Grouping;

namespace QueryLanguage.Scientific
{
    public static class ScientificQueryTableExtensions
    {
        public static INullableQueryTable<T> DewPoint<T>(this INullableQueryTable<T> sourceTable, string temperatureName, string humidityName, string dewPointName="Dew point") where T : struct
        {
            return TempMath(sourceTable, temperatureName, humidityName, dewPointName, m => m.Taupunkt);
        }

        /// <summary>
        /// Absolute Humidity in g/m3
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceTable"></param>
        /// <param name="temperatureName"></param>
        /// <param name="humidityName"></param>
        /// <param name="absoluteHumidityName"></param>
        /// <returns></returns>
        public static INullableQueryTable<T> AbsoluteHumidity<T>(this INullableQueryTable<T> sourceTable, string temperatureName, string humidityName, string absoluteHumidityName = "Absolute humidity") where T : struct
        {
            return TempMath(sourceTable, temperatureName, humidityName, absoluteHumidityName, m => m.AbsoluteHumitity);
        }

        /// <summary>
        /// Humidex of current temperature und humidity. Is unitless but sometimes used with C°
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceTable"></param>
        /// <param name="temperatureName"></param>
        /// <param name="humidityName"></param>
        /// <param name="humidexName"></param>
        /// <returns></returns>
        public static INullableQueryTable<T> Humidex<T>(this INullableQueryTable<T> sourceTable, string temperatureName, string humidityName, string humidexName = "Humidex") where T : struct
        {
            return TempMath(sourceTable, temperatureName, humidityName, humidexName, m => m.Humidex);
        }

        /// <summary>
        /// Calculates the current temperature where percipitation will fall as snow
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceTable"></param>
        /// <param name="humidityName"></param>
        /// <param name="resultName"></param>
        /// <param name="dewPoint">0°C for snow, 0.8°C for sleet</param>
        /// <returns></returns>
        public static INullableQueryTable<T> SnowingTemperature<T>(this INullableQueryTable<T> sourceTable, string humidityName, string resultName = "Snowing Temperature", float dewPoint = 0.8f) where T : struct
        {
            return sourceTable.ToNewTable((o, n) =>
            {
                n.AddSerie(sourceTable.TryGetSerie(humidityName).CalcValue(humidity => CalcSnowingTemperature(dewPoint, humidity), resultName));
            });
        }

        private static T? CalcSnowingTemperature<T>(float dewPoint, T? humidity) where T : struct
        {
            if (humidity.HasValue == false) return null;
            float temp = dewPoint;
            var doubleHumidity = humidity.ToDouble();
            var calc = new TemperatureMath(temp, doubleHumidity);
            temp = (float) (temp + (temp - calc.Taupunkt));

            for (int i = 0; i < 3; i++)
            {
                var calc2 = new TemperatureMath(temp, doubleHumidity);
                temp = (float) (temp + (dewPoint - calc2.Taupunkt));
            }

            return temp.ToType<T>();
        }

        public static INullableQueryTable<T> SnowLine<T>(this INullableQueryTable<T> sourceTable, string temperatureName, string humidityName, float stationAltitude, string resultName = "Snow Line",  float dewPoint = 0f) where T : struct
        {
            return CalcLine(sourceTable, temperatureName, humidityName, resultName, dewPoint, stationAltitude);
        }

        public static INullableQueryTable<T> SleetLine<T>(this INullableQueryTable<T> sourceTable, string temperatureName, string humidityName, float stationAltitude, string resultName = "Sleet Line", float dewPoint = 0.8f) where T : struct
        {
            return CalcLine(sourceTable, temperatureName, humidityName, resultName, dewPoint, stationAltitude);
        }

        private static INullableQueryTable<T> CalcLine<T>(INullableQueryTable<T> sourceTable, string temperatureName, string humidityName, string resultName, float dewPoint, float stationAltitude) where T : struct
        {
            return sourceTable.ToNewTable((o, n) =>
            {
                n.AddSerie(sourceTable.TryGetSerie(temperatureName).Zip(sourceTable.TryGetSerie(humidityName), resultName,
                    (t, h) =>
                    {
                        if (t.HasValue == false || h.HasValue == false) return null;
                        T? snowTemp = CalcSnowingTemperature(dewPoint, h);
                        var altititude = stationAltitude + (t.ToFloat() - snowTemp.ToFloat())*160;
                        return altititude.ToType<T>();
                    }));
            });
        }


        public static INullableQueryTable<T> AddDewPoint<T>(this INullableQueryTable<T> sourceTable, string temperatureName, string humidityName, string dewPointName = "dew point") where T : struct
        {
            return sourceTable.MergeTable(sourceTable.DewPoint(temperatureName, humidityName, dewPointName));
        }
        public static INullableQueryTable<T> AddAbsoluteHumidity<T>(this INullableQueryTable<T> sourceTable, string temperatureName, string humidityName, string absoluteHumidityName = "Absolute humidity") where T : struct
        {
            return sourceTable.MergeTable(sourceTable.AbsoluteHumidity(temperatureName, humidityName, absoluteHumidityName));
        }
        public static INullableQueryTable<T> AddHumidex<T>(this INullableQueryTable<T> sourceTable, string temperatureName, string humidityName, string humidexName = "Humidex") where T : struct
        {
            return sourceTable.MergeTable(sourceTable.Humidex(temperatureName, humidityName, humidexName));
        }
        public static INullableQueryTable<T> AddSnowingTemperature<T>(this INullableQueryTable<T> sourceTable, string humidityName, string resultName = "Snowing Temperature", float dewPoint = 0.8f) where T : struct
        {
            return sourceTable.MergeTable(sourceTable.SnowingTemperature(humidityName, resultName, dewPoint));
        }

        public static INullableQueryTable<T> AddSnowLine<T>(this INullableQueryTable<T> sourceTable, string temperatureName, string humidityName, float stationAltitude, string resultName = "Snow Line", float dewPoint = 0f) where T : struct
        {
            return sourceTable.MergeTable(sourceTable.SnowLine(temperatureName, humidityName, stationAltitude, resultName, dewPoint));
        }

        public static INullableQueryTable<T> AddSleetLine<T>(this INullableQueryTable<T> sourceTable, string temperatureName, string humidityName, float stationAltitude, string resultName = "Sleet Line", float dewPoint = 0.8f) where T : struct
        {
            return sourceTable.MergeTable(sourceTable.SleetLine(temperatureName, humidityName, stationAltitude, resultName, dewPoint));
        }

        private static INullableQueryTable<T> TempMath<T>(INullableQueryTable<T> sourceTable, string temperatureName,
           string humidityName, string dewPointName, Func<TemperatureMath, double> valueSelector) where T : struct
        {
            return sourceTable.ToNewTable((o, n) =>
            {
                n.AddSerie(sourceTable.TryGetSerie(temperatureName).Zip(sourceTable.TryGetSerie(humidityName), dewPointName,
                    (t, h) =>
                    {
                        if (!t.HasValue || !h.HasValue) return null;
                        var tempMath = new TemperatureMath(System.Convert.ToDouble(t.Value), System.Convert.ToDouble(h.Value));
                        return (T)System.Convert.ChangeType(valueSelector(tempMath), typeof(T));
                    }));
            });
        }

    }
}
using DbInterfaces.Interfaces;
using QueryLanguage.Grouping;

namespace QueryLanguage.Scientific
{
    public static class QueryTableExtensions
    {
        public static INullableQueryTable<T> DewPoint<T>(this INullableQueryTable<T> sourceTable, string temperatureName, string humidityName, string dewPointName="dew point") where T : struct
        {
            return sourceTable.ToNewTable((o, n) =>
            {
                n.AddSerie(sourceTable.TryGetSerie(temperatureName).Zip(sourceTable.TryGetSerie(humidityName), dewPointName,
                    (t, h) =>
                    {
                        if (!t.HasValue || !h.HasValue) return null;
                        var tempMath = new TemperatureMath(System.Convert.ToDouble(t.Value), System.Convert.ToDouble(h.Value));
                        return (T) System.Convert.ChangeType(tempMath.Taupunkt, typeof(T));
                    }));
            });
        }

        public static INullableQueryTable<T> AddDewPoint<T>(this INullableQueryTable<T> sourceTable, string temperatureName, string humidityName, string dewPointName = "dew point") where T : struct
        {
            return sourceTable.MergeTable(sourceTable.DewPoint(temperatureName, humidityName, dewPointName));
        }

    }
}
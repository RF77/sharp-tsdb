
using System.Linq;
using MathNet.Numerics.Statistics;

namespace QueryLanguage.Grouping
{
    public static class AggregationDataExtensions
    {
        public static T? First<T>(this AggregationData<T> data) where T:struct
        {
            //if (!data.Rows.Any()) return null;
            return data.Rows.FirstOrDefault()?.Value;
        }

        public static T? Last<T>(this AggregationData<T> data) where T : struct
        {
            return data.Rows.LastOrDefault()?.Value;
        }

        public static T? Max<T>(this AggregationData<T> data) where T : struct
        {
            if (!data.Rows.Any()) return null;
            return data.Rows.Select(i => i.Value).Max();
        }

        public static T? Min<T>(this AggregationData<T> data) where T : struct
        {
            if (!data.Rows.Any()) return null;
            return data.Rows.Select(i => i.Value).Min();
        }

        public static float? Mean(this AggregationData<float> data)
        {
            if (!data.Rows.Any()) return null;
            return (float)data.Rows.Select(i => i.Value).Mean();
        }

    }
}
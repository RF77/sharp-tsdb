
using System.Linq;

namespace QueryLanguage.Grouping
{
    public static class AggregationDataExtensions
    {
        public static T? First<T>(this AggregationData<T> data) where T:struct
        {
            //if (!data.Values.Any()) return null;
            return data.Values.FirstOrDefault()?.Value;
        }

        //public static float? First(this AggregationData<float> data)
        //{
        //    //if (!data.Values.Any()) return null;
        //    return data.Values.FirstOrDefault()?.Value;
        //}
    }
}
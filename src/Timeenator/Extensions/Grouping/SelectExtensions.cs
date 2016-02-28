using System.Linq;
using Timeenator.Extensions.Converting;
using Timeenator.Interfaces;

namespace Timeenator.Extensions.Grouping
{
    public static class SelectExtensions
    {
        public static ISingleDataRow<T> FirstItem<T>(this IQuerySerie<T> data) where T:struct
        {
            //if (!serie.Rows.Any()) return null;
            return data.Rows.FirstOrDefault();
        }

        public static ISingleDataRow<T> LastItem<T>(this IQuerySerie<T> data) where T : struct
        {
            return data.Rows.LastOrDefault();
        }

        public static ISingleDataRow<T> MaxItem<T>(this IQuerySerie<T> data) where T : struct
        {
            if (!data.Rows.Any()) return null;
            var maxItem = data.Rows.First();
            foreach (var row in data.Rows)
            {
                if (row.Value.ToDouble() > maxItem.Value.ToDouble())
                {
                    maxItem = row;
                }
            }
            return maxItem;
        }

        public static ISingleDataRow<T> MinItem<T>(this IQuerySerie<T> data) where T : struct
        {
            if (!data.Rows.Any()) return null;
            var minItem = data.Rows.First();
            foreach (var row in data.Rows)
            {
                if (row.Value.ToDouble() < minItem.Value.ToDouble())
                {
                    minItem = row;
                }
            }
            return minItem;
        }

       
    }
}
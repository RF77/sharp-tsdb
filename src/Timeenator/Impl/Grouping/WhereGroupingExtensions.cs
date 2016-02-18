using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;

namespace QueryLanguage.Grouping
{
    public static class WhereGroupingExtensions
    {
        public static INullableQuerySerie<T> GroupBy<T>(this IQuerySerie<T> serie, IEnumerable<StartEndTime> groupTimes,
            Func<IQuerySerie<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start)
            where T : struct
        {
            var rows = serie.Rows;
            if (!rows.Any() || !groupTimes.Any())
                return new NullableQuerySerie<T>(new List<ISingleDataRow<T?>>(), serie);

            int index = 0;
            int max = rows.Count;

            List<ISingleDataRow<T?>> result = new List<ISingleDataRow<T?>>();

            foreach (var groupTime in groupTimes)
            {
                List<ISingleDataRow<T>> group = new List<ISingleDataRow<T>>();
                while (index < max && rows[index].Time < groupTime.Start)
                {
                    index++;
                }
                var startIndex = index;

                while (index < max && rows[index].Time < groupTime.End)
                {
                    group.Add(rows[index]);

                    index++;
                }
                var aggregationData = new QuerySerie<T>(@group, groupTime.Start, groupTime.End)
                {
                    PreviousRow = startIndex > 0 ? rows[startIndex - 1] : serie.PreviousRow,
                    NextRow = index < max ? rows[index] : serie.NextRow
                };
                result.Add(new SingleDataRow<T?>(groupTime.GetTimeStampByType(timeStampType),
                    aggregationFunc(aggregationData)));
            }

            var resultData = new NullableQuerySerie<T>(result, serie);
            return resultData;
        }
    }
}

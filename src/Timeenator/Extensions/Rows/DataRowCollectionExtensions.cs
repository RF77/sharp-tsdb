using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timeenator.Interfaces;

namespace Timeenator.Extensions.Rows
{
    public static class DataRowCollectionExtensions
    {
        public static IEnumerable<IDataRow> ValueChanges(this IEnumerable<IDataRow> rows)
        {
            IDataRow prev = null;
            foreach (var dataRow in rows)
            {
                if (dataRow.Value.Equals(prev?.Value) == false)
                {
                    yield return dataRow;
                }
                prev = dataRow;
            }
        }

        public static IEnumerable<IDataRow> MinimalTimeSpan(this IEnumerable<IDataRow> rows, string minimalTimeSpan)
        {
            return rows.MinimalTimeSpan(minimalTimeSpan.ToTimeSpan());
        }

        public static IEnumerable<IDataRow> MinimalTimeSpan(this IEnumerable<IDataRow> rows, TimeSpan minimalTimeSpan)
        {
            IDataRow prev = null;
            foreach (var dataRow in rows)
            {
                if ((dataRow.Key - (prev?.Key ?? DateTime.MinValue)) >= minimalTimeSpan)
                {
                    yield return dataRow;
                    prev = dataRow;
                }
            }
        }
    }
}

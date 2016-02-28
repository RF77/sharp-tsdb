using System;
using System.Linq;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping
{
    public static class FillExtensions
    {
        public static INullableQuerySerie<T> FillValue<T>(this INullableQuerySerie<T> serie, T fillValue) where T : struct
        {
            foreach (var row in serie.Rows)
            {
                if (row.Value == null)
                {
                    row.Value = fillValue;
                }
            }
            return serie;
        }

        public static IQuerySerie<T> RemoveNulls<T>(this INullableQuerySerie<T> serie) where T : struct
        {
            return new QuerySerie<T>(serie.Rows.Where(i => i.Value != null).Select(i => new SingleDataRow<T>(i.TimeUtc, i.Value.Value)).ToList(), serie);
        }

        public static INullableQuerySerie<T> Fill<T>(this INullableQuerySerie<T> serie, ValueForNull fillValue) where T : struct
        {
            switch (fillValue)
            {
                case ValueForNull.Previous:
                {
                    T? previous = serie.PreviousRow?.Value;
                    foreach (var row in serie.Rows)
                    {
                        if (row.Value == null)
                        {
                            row.Value = previous;
                        }
                        else
                        {
                            previous = row.Value;
                        }
                    }
                }
                    break;
                case ValueForNull.Next:
                {

                    T? next = serie.NextRow?.Value;
                    var rows = serie.Rows;
                    for (int i = rows.Count - 1; i >= 0; i--)
                    {
                        var item = rows[i];
                        if (item.Value == null)
                        {
                            item.Value = next;
                        }
                        else
                        {
                            next = item.Value;
                        }
                    }
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fillValue), fillValue, null);
            }
            return serie;
        }
    }
}
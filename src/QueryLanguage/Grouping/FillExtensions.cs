using System;
using System.Linq;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;

namespace QueryLanguage.Grouping
{
    public static class FillExtensions
    {
        public static INullableQueryData<T> FillValue<T>(this INullableQueryData<T> data, T fillValue) where T : struct
        {
            foreach (var row in data.Rows)
            {
                if (row.Value == null)
                {
                    row.Value = fillValue;
                }
            }
            return data;
        }

        public static IQueryData<T> RemoveNulls<T>(this INullableQueryData<T> data) where T : struct
        {
            return new QueryData<T>(data.Rows.Where(i => i.Value != null).Select(i => new SingleDataRow<T>(i.Key, i.Value.Value)).ToList(), data);
        }

        public static INullableQueryData<T> Fill<T>(this INullableQueryData<T> data, ValueForNull fillValue) where T : struct
        {
            switch (fillValue)
            {
                case ValueForNull.Previous:
                {
                    T? previous = data.PreviousRow?.Value;
                    foreach (var row in data.Rows)
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

                    T? next = data.NextRow?.Value;
                    var rows = data.Rows;
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
            return data;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;

namespace QueryLanguage.Grouping
{
    public static class TimeGroupingExtensions
    {
        public static INullableQueryData<T> FillValue<T>(this NullableQueryData<T> data, T fillValue) where T : struct
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

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static NullableQueryData<T> GroupByMinutes<T>(this IQueryData<T> data, int minutes,
            Func<AggregationData<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            ISingleDataRow<T> first = data.Rows.First();
            DateTime d = data.StartTime ?? first.Key;

            int startMinute = d.Minute;
            if (60 % minutes == 0)
            {
                //change start minute to even minute
                startMinute = startMinute - (startMinute % minutes);
            }

            DateTime currentDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, startMinute, 0);

            return data.GroupByTime(0, currentDate, data.StopTime, dt => dt + TimeSpan.FromMinutes(minutes), aggregationFunc, timeStampType);
        }

        public static NullableQueryData<T> GroupByTime<T>(this IQueryData<T> data,
            int currentIndex, DateTime startTime, DateTime? stopTime, Func<DateTime, DateTime> calcNewDateMethod,
            Func<AggregationData<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            DateTime endTime = calcNewDateMethod(startTime);
            ISingleDataRow<T> previous = null;
            List<ISingleDataRow<T?>> result = new List<ISingleDataRow<T?>>();
            var resultData = new NullableQueryData<T>(result, data);
            var items = data.Rows;
            do
            {
                List<ISingleDataRow<T>> list = new List<ISingleDataRow<T>>();
                while (currentIndex < items.Count && items[currentIndex].Key < endTime)
                {
                    list.Add(items[currentIndex++]);
                }
                ISingleDataRow<T> next;
                if (currentIndex < items.Count)
                {
                    next = items[currentIndex];
                }
                else
                {
                    next = null;
                }

                var aggregationData = new AggregationData<T>
                {
                    Next = next,
                    Previous = previous,
                    Rows = list,
                    StartTime = startTime,
                    EndTime = endTime,
                };
                T? value = aggregationFunc(aggregationData);

                DateTime timeStamp = startTime;
                if (timeStampType == TimeStampType.End)
                {
                    timeStamp = endTime;
                }
                else if (timeStampType == TimeStampType.Middle)
                {
                    timeStamp = startTime + TimeSpan.FromMinutes((endTime - startTime).TotalMinutes / 2);
                }

                result.Add(new SingleDataRow<T?>(timeStamp, value));

                if (currentIndex > 0) previous = items[currentIndex - 1];

                startTime = endTime;
                endTime = calcNewDateMethod(startTime);
            } while (currentIndex < items.Count || (stopTime != null && startTime < stopTime));
            return resultData;
        }
    }
}

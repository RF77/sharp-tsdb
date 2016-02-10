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
        public static IEnumerable<ISingleDataRow<T?>> FillValue<T>(this IEnumerable<ISingleDataRow<T?>> rows, T fillValue) where T : struct
        {
            foreach (var row in rows)
            {
                if (row.Value == null)
                {
                    row.Value = fillValue;
                }
                yield return row;
            }
        }

        public static IEnumerable<ISingleDataRow<T>> RemoveNulls<T>(this IEnumerable<ISingleDataRow<T?>> rows) where T : struct
        {
            return rows.Where(i => i.Value != null).Select(i => new SingleDataRow<T>(i.Key, i.Value.Value));
        }

        public static IList<ISingleDataRow<T?>> Fill<T>(this IEnumerable<ISingleDataRow<T?>> rows, ValueForNull fillValue) where T : struct
        {
            var rowList = rows.ToList();
            switch (fillValue)
            {
                case ValueForNull.Previous:
                    {
                        T? previous = null;
                        foreach (var row in rowList)
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
                        
                        T? next = null;
                        for (int i = rowList.Count - 1; i >= 0; i--)
                        {
                            var item = rowList[i];
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
            return rowList;
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static IEnumerable<ISingleDataRow<T?>> GroupByMinutes<T>(this IQueryData<T> data, int minutes,
            Func<AggregationData<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            //if (rows == null || !rows.Any())
            //{
            //    return Enumerable.Empty<ISingleDataRow<T?>>();
            //}
            List<ISingleDataRow<T>> rowList = data.Rows.ToList();

            ISingleDataRow<T> first = rowList.First();
            DateTime d = data.StartTime ?? first.Key;

            int startMinute = d.Minute;
            if (60 % minutes == 0)
            {
                //change start minute to even minute
                startMinute = startMinute - (startMinute % minutes);
            }

            DateTime currentDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, startMinute, 0);

            return rowList.GroupByTime(0, currentDate, data.StopTime, dt => dt + TimeSpan.FromMinutes(minutes), aggregationFunc, timeStampType);
        }

        public static IEnumerable<ISingleDataRow<T?>> GroupByTime<T>(this IList<ISingleDataRow<T>> items,
            int currentIndex, DateTime startTime, DateTime? stopTime, Func<DateTime, DateTime> calcNewDateMethod,
            Func<AggregationData<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            DateTime endTime = calcNewDateMethod(startTime);
            ISingleDataRow<T> previous = null;

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

                var singleDataRow = new SingleDataRow<T?>(timeStamp, value);
                yield return singleDataRow;

                if (currentIndex > 0) previous = items[currentIndex - 1];

                startTime = endTime;
                endTime = calcNewDateMethod(startTime);
            } while (currentIndex < items.Count || (stopTime != null && startTime < stopTime));
        }
    }
}

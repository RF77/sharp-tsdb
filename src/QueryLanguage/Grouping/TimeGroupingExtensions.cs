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
        internal static T? FillValue<T>(AggregationData<T> data, ValueForNull fillValue) where T : struct
        {
            switch (fillValue)
            {
                case ValueForNull.Null:
                    return null;
                case ValueForNull.Zero:
                    var changeType = Convert.ChangeType(typeof(T), 0);
                    if (changeType != null) return (T)changeType;
                    return null;
                case ValueForNull.Previous:
                    return data.Previous.Value;
                case ValueForNull.Next:
                    return data.Next.Value;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fillValue), fillValue, null);
            }
        }
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static IEnumerable<ISingleDataRow<T?>> GroupByMinutes<T>(this IEnumerable<ISingleDataRow<T>> rows, int minutes,
            Func<AggregationData<T>, T?> aggregationFunc,
            ValueForNull fillValue = ValueForNull.Null, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (rows == null || !rows.Any())
            {
                return Enumerable.Empty<ISingleDataRow<T?>>();
            }
            List<ISingleDataRow<T>> rowList = rows.ToList();

            ISingleDataRow<T> first = rowList.First();
            DateTime d = first.Key;

            int startMinute = d.Minute;
            if (60%minutes == 0)
            {
                //change start minute to even minute
                startMinute = startMinute - (startMinute%minutes);
            }

            DateTime currentDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, startMinute, 0);

            return rowList.GroupByTime(0,currentDate, dt => dt + TimeSpan.FromMinutes(minutes), aggregationFunc, fillValue, timeStampType);
        }

        public static IEnumerable<ISingleDataRow<T?>> GroupByTime<T>(this IList<ISingleDataRow<T>> items,
            int currentIndex, DateTime startTime, Func<DateTime, DateTime> calcNewDateMethod,
            Func<AggregationData<T>, T?> aggregationFunc,
            ValueForNull fillValue = ValueForNull.Null, TimeStampType timeStampType = TimeStampType.Start) where T:struct
        {
            DateTime endTime = calcNewDateMethod(startTime);
            ISingleDataRow<T> previous = null;

            do
            {
                List< ISingleDataRow < T >> list = new List<ISingleDataRow<T>>();
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
                    Values = list,
                    StartTime = startTime,
                    EndTime = endTime,
                };
                var value = aggregationFunc(aggregationData);

                DateTime timeStamp = startTime;
                if (timeStampType == TimeStampType.End)
                {
                    timeStamp = endTime;
                }
                else if (timeStampType == TimeStampType.Middle)
                {
                    timeStamp = startTime + TimeSpan.FromMinutes((endTime - startTime).TotalMinutes/2);
                }

                yield return new SingleDataRow<T?> (timeStamp, value ?? FillValue(aggregationData, fillValue));

                if (currentIndex > 0)
                {
                    previous = items[currentIndex - 1];
                }

                startTime = endTime;
                endTime = calcNewDateMethod(startTime);
            } while (currentIndex < items.Count);
        }
    }
}

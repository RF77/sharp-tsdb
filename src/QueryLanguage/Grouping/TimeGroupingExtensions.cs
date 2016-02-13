using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DbInterfaces.Interfaces;
using FileDb.InterfaceImpl;

namespace QueryLanguage.Grouping
{
    public static class TimeGroupingExtensions
    {
        public static INullableQueryData<T> GroupBy<T>(this IQueryData<T> data, string expression,
             Func<AggregationData<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            expression = expression.Trim();
            var match = Regex.Match(expression, "^(\\d+)([smhdwMy])$");
            if (match.Success == false)
            {
                throw new ArgumentException($"expression {expression} is invalid");
            }
            int number = int.Parse(match.Groups[1].Value);
            string type = match.Groups[2].Value;

            switch (type)
            {
                case "s":
                    return data.GroupBySeconds(number, aggregationFunc);
                case "m":
                    return data.GroupByMinutes(number, aggregationFunc);
                case "h":
                    return data.GroupByHours(number, aggregationFunc);
                case "d":
                    return data.GroupByDays(number, aggregationFunc);
                case "w":
                    return data.GroupByWeeks(number, aggregationFunc);
                case "M":
                    return data.GroupByMonths(number, aggregationFunc);
                case "y":
                    return data.GroupByYears(number, aggregationFunc);
            }

            throw new ArgumentException($"expression {expression} has unknown type");
        }

        public static INullableQueryData<T> GroupBySeconds<T>(this IQueryData<T> data, int seconds,
             Func<AggregationData<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (!data.Rows.Any()) return new NullableQueryData<T>(new List<ISingleDataRow<T?>>(), data);
            ISingleDataRow<T> first = data.Rows.First();
            DateTime d = data.StartTime ?? first.Key;

            int startSeconds = d.Second;
            if (60 % seconds == 0)
            {
                //change start minute to even minute
                startSeconds = startSeconds - (startSeconds % seconds);
            }

            DateTime currentDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, startSeconds);

            return data.GroupByTime(0, currentDate, data.StopTime, dt => dt + TimeSpan.FromSeconds(seconds), aggregationFunc, timeStampType);
        }

        public static INullableQueryData<T> GroupByMinutes<T>(this IQueryData<T> data, int minutes,
            Func<AggregationData<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (!data.Rows.Any()) return new NullableQueryData<T>(new List<ISingleDataRow<T?>>(), data);
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

        public static INullableQueryData<T> GroupByHours<T>(this IQueryData<T> data, int hours,
     Func<AggregationData<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (!data.Rows.Any()) return new NullableQueryData<T>(new List<ISingleDataRow<T?>>(), data);
            ISingleDataRow<T> first = data.Rows.First();
            DateTime d = data.StartTime ?? first.Key;

            int startHour = d.Hour;
            if (24 % hours == 0)
            {
                //change start minute to even minute
                startHour = startHour - (startHour % hours);
            }

            DateTime currentDate = new DateTime(d.Year, d.Month, d.Day, startHour, 0, 0);

            return data.GroupByTime(0, currentDate, data.StopTime, dt => dt + TimeSpan.FromHours(hours), aggregationFunc, timeStampType);
        }

        public static INullableQueryData<T> GroupByDays<T>(this IQueryData<T> data, int days,
     Func<AggregationData<T>, T?> aggregationFunc, int startHour = 0, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (!data.Rows.Any()) return new NullableQueryData<T>(new List<ISingleDataRow<T?>>(), data);
            ISingleDataRow<T> first = data.Rows.First();
            DateTime d = data.StartTime ?? first.Key;

            DateTime currentDate = new DateTime(d.Year, d.Month, d.Day, startHour, 0, 0);

            return data.GroupByTime(0, currentDate, data.StopTime, dt => dt + TimeSpan.FromDays(days), aggregationFunc, timeStampType);
        }

        public static INullableQueryData<T> GroupByWeeks<T>(this IQueryData<T> data, int weeks,
Func<AggregationData<T>, T?> aggregationFunc, DayOfWeek startDay = DayOfWeek.Monday, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (!data.Rows.Any()) return new NullableQueryData<T>(new List<ISingleDataRow<T?>>(), data);
            ISingleDataRow<T> first = data.Rows.First();
            DateTime d = data.StartTime ?? first.Key;

            DateTime startDate = new DateTime(d.Year, d.Month, d.Day);

            while (startDate.DayOfWeek != startDay)
            {
                startDate = startDate - TimeSpan.FromDays(1);
            }

            return data.GroupByTime(0, startDate, data.StopTime, dt => dt + TimeSpan.FromDays(weeks * 7), aggregationFunc, timeStampType);
        }

        public static INullableQueryData<T> GroupByMonths<T>(this IQueryData<T> data, int months,
Func<AggregationData<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (!data.Rows.Any()) return new NullableQueryData<T>(new List<ISingleDataRow<T?>>(), data);
            ISingleDataRow<T> first = data.Rows.First();
            DateTime d = data.StartTime ?? first.Key;

            int startMonth = d.Month;
            if (12 % months == 0)
            {
                startMonth = startMonth - (startMonth % months) + 1;
            }

            DateTime currentDate = new DateTime(d.Year, startMonth, 1, 0, 0, 0);

            return data.GroupByTime(0, currentDate, data.StopTime, dt =>
            {
                int year = dt.Year;
                int month = dt.Month;
                int newMonth = month + months;
                year += (newMonth - 1) / 12;
                month = ((newMonth - 1)%12) + 1;
                return new DateTime(year, month, 1);
            }, aggregationFunc, timeStampType);
        }

        public static INullableQueryData<T> GroupByYears<T>(this IQueryData<T> data, int years,
Func<AggregationData<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (!data.Rows.Any()) return new NullableQueryData<T>(new List<ISingleDataRow<T?>>(), data);
            ISingleDataRow<T> first = data.Rows.First();
            DateTime d = data.StartTime ?? first.Key;

            DateTime currentDate = new DateTime(d.Year, 1, 1);

            return data.GroupByTime(0, currentDate, data.StopTime, dt => new DateTime(dt.Year + years, 1, 1), aggregationFunc, timeStampType);
        }

        public static INullableQueryData<T> GroupByTime<T>(this IQueryData<T> data,
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

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
        public static INullableQuerySerie<T> GroupBy<T>(this IQuerySerie<T> serie, string expression,
             Func<IQuerySerie<T>, T?> aggregationFunc, string minIntervalExpression = null, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            if (minIntervalExpression != null)
            {
                var minTimeSpan = minIntervalExpression.ToTimeSpan();
                var currentTimeSpan = expression.ToTimeSpan();
                if (currentTimeSpan < minTimeSpan)
                {
                    expression = minIntervalExpression;
                }
            }
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
                    return serie.GroupBySeconds(number, aggregationFunc);
                case "m":
                    return serie.GroupByMinutes(number, aggregationFunc);
                case "h":
                    return serie.GroupByHours(number, aggregationFunc);
                case "d":
                    return serie.GroupByDays(number, aggregationFunc);
                case "w":
                    return serie.GroupByWeeks(number, aggregationFunc);
                case "M":
                    return serie.GroupByMonths(number, aggregationFunc);
                case "y":
                    return serie.GroupByYears(number, aggregationFunc);
            }

            throw new ArgumentException($"expression {expression} has unknown type");
        }

        public static INullableQuerySerie<T> GroupBySeconds<T>(this IQuerySerie<T> serie, int seconds,
             Func<IQuerySerie<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (!serie.Rows.Any()) return new NullableQuerySerie<T>(new List<ISingleDataRow<T?>>(), serie);
            ISingleDataRow<T> first = serie.Rows.First();
            DateTime d = serie.StartTime ?? first.Time;

            int startSeconds = d.Second;
            if (60 % seconds == 0)
            {
                //change start minute to even minute
                startSeconds = startSeconds - (startSeconds % seconds);
            }

            DateTime currentDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, startSeconds);

            return serie.GroupByTime(0, currentDate, serie.EndTime, dt => dt + TimeSpan.FromSeconds(seconds), aggregationFunc, timeStampType);
        }

        public static INullableQuerySerie<T> GroupByMinutes<T>(this IQuerySerie<T> serie, int minutes,
            Func<IQuerySerie<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (!serie.Rows.Any()) return new NullableQuerySerie<T>(new List<ISingleDataRow<T?>>(), serie);
            ISingleDataRow<T> first = serie.Rows.First();
            DateTime d = serie.StartTime ?? first.Time;

            int startMinute = d.Minute;
            if (60 % minutes == 0)
            {
                //change start minute to even minute
                startMinute = startMinute - (startMinute % minutes);
            }

            DateTime currentDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, startMinute, 0);

            return serie.GroupByTime(0, currentDate, serie.EndTime, dt => dt + TimeSpan.FromMinutes(minutes), aggregationFunc, timeStampType);
        }

        public static INullableQuerySerie<T> GroupByHours<T>(this IQuerySerie<T> serie, int hours,
     Func<IQuerySerie<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (!serie.Rows.Any()) return new NullableQuerySerie<T>(new List<ISingleDataRow<T?>>(), serie);
            ISingleDataRow<T> first = serie.Rows.First();
            DateTime d = serie.StartTime ?? first.Time;

            int startHour = d.Hour;
            if (24 % hours == 0)
            {
                //change start minute to even minute
                startHour = startHour - (startHour % hours);
            }

            DateTime currentDate = new DateTime(d.Year, d.Month, d.Day, startHour, 0, 0);

            return serie.GroupByTime(0, currentDate, serie.EndTime, dt => dt + TimeSpan.FromHours(hours), aggregationFunc, timeStampType);
        }

        public static INullableQuerySerie<T> GroupByDays<T>(this IQuerySerie<T> serie, int days,
     Func<IQuerySerie<T>, T?> aggregationFunc, int startHour = 0, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (!serie.Rows.Any()) return new NullableQuerySerie<T>(new List<ISingleDataRow<T?>>(), serie);
            ISingleDataRow<T> first = serie.Rows.First();
            DateTime d = serie.StartTime ?? first.Time;

            DateTime currentDate = new DateTime(d.Year, d.Month, d.Day, startHour, 0, 0);

            return serie.GroupByTime(0, currentDate, serie.EndTime, dt => dt + TimeSpan.FromDays(days), aggregationFunc, timeStampType);
        }

        public static INullableQuerySerie<T> GroupByWeeks<T>(this IQuerySerie<T> serie, int weeks,
Func<IQuerySerie<T>, T?> aggregationFunc, DayOfWeek startDay = DayOfWeek.Monday, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (!serie.Rows.Any()) return new NullableQuerySerie<T>(new List<ISingleDataRow<T?>>(), serie);
            ISingleDataRow<T> first = serie.Rows.First();
            DateTime d = serie.StartTime ?? first.Time;

            DateTime startDate = new DateTime(d.Year, d.Month, d.Day);

            while (startDate.DayOfWeek != startDay)
            {
                startDate = startDate - TimeSpan.FromDays(1);
            }

            return serie.GroupByTime(0, startDate, serie.EndTime, dt => dt + TimeSpan.FromDays(weeks * 7), aggregationFunc, timeStampType);
        }

        public static INullableQuerySerie<T> GroupByMonths<T>(this IQuerySerie<T> serie, int months,
Func<IQuerySerie<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (!serie.Rows.Any()) return new NullableQuerySerie<T>(new List<ISingleDataRow<T?>>(), serie);
            ISingleDataRow<T> first = serie.Rows.First();
            DateTime d = serie.StartTime ?? first.Time;

            int startMonth = d.Month;
            if (12 % months == 0)
            {
                startMonth = startMonth - (startMonth % months) + 1;
            }

            DateTime currentDate = new DateTime(d.Year, startMonth, 1, 0, 0, 0);

            return serie.GroupByTime(0, currentDate, serie.EndTime, dt =>
            {
                int year = dt.Year;
                int month = dt.Month;
                int newMonth = month + months;
                year += (newMonth - 1) / 12;
                month = ((newMonth - 1)%12) + 1;
                return new DateTime(year, month, 1);
            }, aggregationFunc, timeStampType);
        }

        public static INullableQuerySerie<T> GroupByYears<T>(this IQuerySerie<T> serie, int years,
Func<IQuerySerie<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            if (!serie.Rows.Any()) return new NullableQuerySerie<T>(new List<ISingleDataRow<T?>>(), serie);
            ISingleDataRow<T> first = serie.Rows.First();
            DateTime d = serie.StartTime ?? first.Time;

            DateTime currentDate = new DateTime(d.Year, 1, 1);

            return serie.GroupByTime(0, currentDate, serie.EndTime, dt => new DateTime(dt.Year + years, 1, 1), aggregationFunc, timeStampType);
        }

        public static INullableQuerySerie<T> GroupByTime<T>(this IQuerySerie<T> serie,
            int currentIndex, DateTime startTime, DateTime? stopTime, Func<DateTime, DateTime> calcNewDateMethod,
            Func<IQuerySerie<T>, T?> aggregationFunc, TimeStampType timeStampType = TimeStampType.Start) where T : struct
        {
            DateTime endTime = calcNewDateMethod(startTime);
            ISingleDataRow<T> previous = null;
            List<ISingleDataRow<T?>> result = new List<ISingleDataRow<T?>>();
            var resultData = new NullableQuerySerie<T>(result, serie);
            var items = serie.Rows;
            do
            {
                List<ISingleDataRow<T>> list = new List<ISingleDataRow<T>>();
                while (currentIndex < items.Count && items[currentIndex].Time < endTime)
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

                var aggregationData = new QuerySerie<T>(list, serie)
                {
                    NextRow = next,
                    PreviousRow = previous,
                    StartTime = startTime,
                    EndTime = endTime,
                    LastRow = list.Any() ? list.Last() : null
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

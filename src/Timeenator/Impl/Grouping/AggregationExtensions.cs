using System;
using System.Linq;
using MathNet.Numerics.Statistics;
using Timeenator.Impl.Converting;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping
{
    public static class AggregationExtensions
    {
        public static T? First<T>(this IQuerySerie<T> data) where T:struct
        {
            //if (!serie.Rows.Any()) return null;
            return data.Rows.FirstOrDefault()?.Value;
        }

        public static T? Last<T>(this IQuerySerie<T> data) where T : struct
        {
            return data.Rows.LastOrDefault()?.Value;
        }

        public static T? Max<T>(this IQuerySerie<T> data) where T : struct
        {
            if (!data.Rows.Any()) return null;
            return data.Rows.Select(i => i.Value).Max();
        }

        public static T? Min<T>(this IQuerySerie<T> data) where T : struct
        {
            if (!data.Rows.Any()) return null;
            return data.Rows.Select(i => i.Value).Min();
        }

        /// <summary>
        /// Mean of all measurement points without looking to timestamps
        /// </summary>
        /// <param name="serie"></param>
        /// <returns></returns>
        public static T? Mean<T>(this IQuerySerie<T> serie) where T:struct 
        {
            if (!serie.Rows.Any()) return null;
            return serie.Rows.Select(i => i.Value.ToDouble()).Mean().ToType<T>();
        }

        /// <summary>
        /// Mean of all measurement points with taking the time into account
        /// </summary>
        /// <param name="serie"></param>
        /// <returns></returns>
        public static T? MeanByTime<T>(this IQuerySerie<T> serie) where T:struct 
        {
            if (!serie.Rows.Any()) return null;
            double valueSum = 0;
            var rows = serie.Rows;
            DateTime start = DateTime.MinValue;
            DateTime stop = rows.Last().Time;
            DateTime? currentTimeStamp = null;


            double currentValue = 0;
            if (serie.PreviousRow != null && serie.StartTime != null)
            {
                start = serie.StartTime.Value;
                currentValue = serie.PreviousRow.Value.ToDouble();
                currentTimeStamp = start;
            }

            for (int i = 0; i < rows.Count; i++)
            {
                var newRow = rows[i];
                if (currentTimeStamp != null)
                {
                    valueSum += (newRow.Time - currentTimeStamp.Value).Ticks*currentValue;
                }
                else
                {
                    start = newRow.Time;
                }
                currentValue = newRow.Value.ToDouble();
                currentTimeStamp = newRow.Time;
            }

            if (serie.NextRow != null && serie.EndTime != null)
            {
                stop = serie.EndTime.Value;
                if (currentTimeStamp != null)
                {
                    valueSum += (stop - currentTimeStamp.Value).Ticks * currentValue;
                }
            }

            double result = valueSum / (stop - start).Ticks;

            return result.ToType<T>();
        }

        public static T? Difference<T>(this IQuerySerie<T> serie) where T : struct
        {
            if (!serie.Rows.Any()) return null;
            return (serie.Rows.Last().Value.ToDouble() - (serie.PreviousRow?.Value.ToDouble() ?? serie.Rows.First().Value.ToDouble())).ToType<T>();
        }

        public static T? Median<T>(this IQuerySerie<T> serie) where T : struct
        {
            if (!serie.Rows.Any()) return null;
            return serie.Rows.Select(i => i.Value.ToDouble()).Median().ToType<T>();
        }

        public static T? Sum<T>(this IQuerySerie<T> serie) where T : struct
        {
            if (!serie.Rows.Any()) return null;
            return serie.Rows.Select(i => i.Value.ToDouble()).Sum().ToType<T>();
        }

        /// <summary>
        /// Calculates the time span where a specified condition (predicate) is true
        /// e.g. serie.TimeWhere(v => v == 9.6f)?.TotalMinutes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serie"></param>
        /// <param name="predicate">true, if added to time span</param>
        /// <returns>Time Span / use for example TotalMinutes to get a value of type T again</returns>
        public static TimeSpan? TimeWhere<T>(this IQuerySerie<T> serie, Func<T, bool> predicate) where T:struct 
        {
            if (!serie.Rows.Any()) return null;
            TimeSpan? timeSpan = TimeSpan.Zero;
            var rows = serie.Rows;

            ISingleDataRow<T> prevRow = null;

            for (int i = 0; i < rows.Count; i++)
            {
                ISingleDataRow<T> newRow = rows[i];
                if (i == 0 && serie.PreviousRow != null && serie.StartTime != null)
                {
                    if (predicate(serie.PreviousRow.Value))
                    {
                        timeSpan += (newRow.Time - serie.StartTime.Value);
                    }
                }
                else
                {
                    if (prevRow != null && predicate(prevRow.Value))
                    {
                        timeSpan += (newRow.Time - prevRow.Time);
                    }
                }

                prevRow = newRow;
            }

            if (serie.NextRow != null && serie.EndTime != null)
            {
                if (predicate(prevRow.Value))
                {
                    timeSpan += (serie.EndTime.Value - prevRow.Time);
                }
            }

            return timeSpan;
        }
    }
}
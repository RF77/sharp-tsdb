using System;
using System.Linq;
using MathNet.Numerics.Statistics;
using Timeenator.Extensions;
using Timeenator.Extensions.Converting;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    public partial class QuerySerie<T> where T:struct 
    {
        public T? First()
        {
            //if (!Rows.Any()) return null;
            return Rows.FirstOrDefault()?.Value;
        }

        public T? Last()
        {
            return Rows.LastOrDefault()?.Value;
        }

        public T? Max()
        {
            if (!Rows.Any()) return null;
            return Rows.Select(i => i.Value).Max();
        }

        public T? Min()
        {
            if (!Rows.Any()) return null;
            return Rows.Select(i => i.Value).Min();
        }

        /// <summary>
        /// Mean of all measurement points without looking to timestamps
        /// </summary>
        /// <returns></returns>
        public T? Mean()
        {
            if (!Rows.Any()) return null;
            return Rows.Select(i => i.Value.ToDouble()).Mean().ToType<T>();
        }

        /// <summary>
        /// Mean of all measurement points with taking the time into account
        /// </summary>
        /// <returns></returns>
        public T? MeanByTime()
        {
            if (!Rows.Any()) return null;
            double valueSum = 0;
            var rows = Rows;
            DateTime start = DateTime.MinValue;
            DateTime stop = rows.Last().TimeUtc;
            DateTime? currentTimeStamp = null;


            double currentValue = 0;
            if (PreviousRow != null && StartTime != null)
            {
                start = StartTime.Value;
                currentValue = PreviousRow.Value.ToDouble();
                currentTimeStamp = start;
            }

            for (int i = 0; i < rows.Count; i++)
            {
                var newRow = rows[i];
                if (currentTimeStamp != null)
                {
                    valueSum += (newRow.TimeUtc - currentTimeStamp.Value).Ticks * currentValue;
                }
                else
                {
                    start = newRow.TimeUtc;
                }
                currentValue = newRow.Value.ToDouble();
                currentTimeStamp = newRow.TimeUtc;
            }

            if (NextRow != null && EndTime != null)
            {
                stop = EndTime.Value;
                if (currentTimeStamp != null)
                {
                    valueSum += (stop - currentTimeStamp.Value).Ticks * currentValue;
                }
            }

            double result = valueSum / (stop - start).Ticks;

            return result.ToType<T>();
        }

        public T? Difference()
        {
            if (!Rows.Any()) return null;
            return (Rows.Last().Value.ToDouble() - (PreviousRow?.Value.ToDouble() ?? Rows.First().Value.ToDouble())).ToType<T>();
        }

        public T? Derivative(TimeSpan timeSpan)
        {
            if (!Rows.Any()) return null;
            var firstValue = PreviousRow ?? Rows.First();
            var lastValue = Rows.Last();
            var diffTime = lastValue.TimeUtc - firstValue.TimeUtc;
            double diffValue = lastValue.Value.ToDouble() - firstValue.Value.ToDouble();
            var result = diffValue * timeSpan.Ticks / diffTime.Ticks;
            return result.ToType<T>();
        }

        public T? Derivative(string timeSpanExpression)
        {
            return Derivative(timeSpanExpression.ToTimeSpan());
        }

        public T? Median()
        {
            if (!Rows.Any()) return null;
            return Rows.Select(i => i.Value.ToDouble()).Median().ToType<T>();
        }

        public T? Sum()
        {
            if (!Rows.Any()) return null;
            return Rows.Select(i => i.Value.ToDouble()).Sum().ToType<T>();
        }

        /// <summary>
        /// Calculates the time span where a specified condition (predicate) is true
        /// e.g. TimeWhere(v => v == 9.6f)?.TotalMinutes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">true, if added to time span</param>
        /// <returns>Time Span / use for example TotalMinutes to get a value of type T again</returns>
        public TimeSpan? TimeWhere(Func<T, bool> predicate)
        {
            if (!Rows.Any()) return null;
            TimeSpan? timeSpan = TimeSpan.Zero;
            var rows = Rows;

            ISingleDataRow<T> prevRow = null;

            for (int i = 0; i < rows.Count; i++)
            {
                ISingleDataRow<T> newRow = rows[i];
                if (i == 0 && PreviousRow != null && StartTime != null)
                {
                    if (predicate(PreviousRow.Value))
                    {
                        timeSpan += (newRow.TimeUtc - StartTime.Value);
                    }
                }
                else
                {
                    if (prevRow != null && predicate(prevRow.Value))
                    {
                        timeSpan += (newRow.TimeUtc - prevRow.TimeUtc);
                    }
                }

                prevRow = newRow;
            }

            if (NextRow != null && EndTime != null)
            {
                if (prevRow != null && predicate(prevRow.Value))
                {
                    timeSpan += (EndTime.Value - prevRow.TimeUtc);
                }
            }

            return timeSpan;
        }
    }
}

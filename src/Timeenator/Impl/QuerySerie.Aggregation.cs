// /*******************************************************************************
//  * Copyright (c) 2016 by RF77 (https://github.com/RF77)
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the Eclipse Public License v1.0
//  * which accompanies this distribution, and is available at
//  * http://www.eclipse.org/legal/epl-v10.html
//  *
//  * Contributors:
//  *    RF77 - initial API and implementation and/or initial documentation
//  *******************************************************************************/ 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.Statistics;
using Timeenator.Extensions;
using Timeenator.Extensions.Converting;
using Timeenator.Interfaces;

namespace Timeenator.Impl
{
    public partial class QuerySerie<T> where T : struct
    {
        private const int NumberOfExpFactors = 20;
        private static readonly double[] ExponentialFactors;
        private static readonly double ExpSum;

        static QuerySerie()
        {
            var b = Math.Exp(1);

            IList<double> eFactors = new List<double>(NumberOfExpFactors);
            for (double j = 0; j < NumberOfExpFactors; j++)
            {
                eFactors.Add(b/Math.Exp(1 + (j/7)));
            }
            ExponentialFactors = eFactors.Reverse().Concat(eFactors.Skip(1)).ToArray();
            ExpSum = ExponentialFactors.Sum();
        }

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
        ///     Mean of all measurement points without looking to timestamps
        /// </summary>
        /// <returns></returns>
        public T? Mean()
        {
            if (!Rows.Any()) return null;
            return Rows.Select(i => i.Value.ToDouble()).Mean().ToType<T>();
        }

        public T? MeanByTimeIncludePreviousAndNext()
        {
            return MeanByTime(true);
        }

        public T? MeanByTime()
        {
            return MeanByTime(false);
        }

        public T? MeanExpWeighted()
        {
            if (!Rows.Any()) return null;
            var rows = Rows;
            DateTime start = DateTime.MinValue;
            DateTime stop = rows.Last().TimeUtc;

            if (StartTime != null)
            {
                start = StartTime.Value;
            }

            if (NextRow != null && EndTime != null)
            {
                stop = EndTime.Value;
            }

            var diff = stop - start;
            var timeSpanSubGroup = TimeSpan.FromTicks((diff.Ticks + (NumberOfExpFactors*2))/ExponentialFactors.Length);
            var newSerie = new QuerySerie<T>(Rows, start, stop) {PreviousRow = PreviousRow, NextRow = NextRow};
            var subGroups =
                newSerie.Group(g => g.ByTime.Span(timeSpanSubGroup).Aggregate(f => f.MeanByTimeIncludePreviousAndNext()))
                    .RemoveNulls();
            Debug.Assert(subGroups.Rows.Count == ExponentialFactors.Length);
            var subRows = subGroups.Rows;

            double sum = 0;

            for (int i = 0; i < subRows.Count; i++)
            {
                sum += subRows[i].Value.ToDouble()*ExponentialFactors[i];
            }

            double result = sum/ExpSum;

            return result.ToType<T>();
        }

        public T? Difference()
        {
            if (!Rows.Any()) return null;
            return
                (Rows.Last().Value.ToDouble() - (PreviousRow?.Value.ToDouble() ?? Rows.First().Value.ToDouble()))
                    .ToType<T>();
        }

        public T? Derivative(TimeSpan timeSpan)
        {
            if (!Rows.Any()) return null;
            var firstValue = PreviousRow ?? Rows.First();
            var lastValue = Rows.Last();
            var diffTime = lastValue.TimeUtc - firstValue.TimeUtc;
            double diffValue = lastValue.Value.ToDouble() - firstValue.Value.ToDouble();
            var result = diffValue*timeSpan.Ticks/diffTime.Ticks;
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
        ///     Calculates the time span where a specified condition (predicate) is true
        ///     e.g. TimeWhere(v => v == 9.6f)?.TotalMinutes
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

        public ISingleDataRow<T> FirstItem()
        {
            //if (!serie.Rows.Any()) return null;
            return Rows.FirstOrDefault();
        }

        public ISingleDataRow<T> LastItem()
        {
            return Rows.LastOrDefault();
        }

        public ISingleDataRow<T> MaxItem()
        {
            if (!Rows.Any()) return null;
            var maxItem = Rows.First();
            foreach (var row in Rows)
            {
                if (row.Value.ToDouble() > maxItem.Value.ToDouble())
                {
                    maxItem = row;
                }
            }
            return maxItem;
        }

        public ISingleDataRow<T> MinItem()
        {
            if (!Rows.Any()) return null;
            var minItem = Rows.First();
            foreach (var row in Rows)
            {
                if (row.Value.ToDouble() < minItem.Value.ToDouble())
                {
                    minItem = row;
                }
            }
            return minItem;
        }

        /// <summary>
        ///     Mean of all measurement points with taking the time into account
        /// </summary>
        /// <returns></returns>
        private T? MeanByTime(bool includePreviousAndNext)
        {
            var notAnyRows = !Rows.Any();
            if (notAnyRows && (!includePreviousAndNext || (PreviousRow == null && NextRow == null))) return null;
            if (notAnyRows)
            {
                return PreviousRow?.Value ?? NextRow.Value;
            }

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
                    valueSum += (newRow.TimeUtc - currentTimeStamp.Value).Ticks*currentValue;
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
                    valueSum += (stop - currentTimeStamp.Value).Ticks*currentValue;
                }
            }

            double result = valueSum/(stop - start).Ticks;

            return result.ToType<T>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using DbInterfaces.Interfaces;
using MathNet.Numerics.Statistics;
using QueryLanguage.Converting;

namespace QueryLanguage.Grouping
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
            DateTime stop = rows.Last().Key;
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
                    valueSum += (newRow.Key - currentTimeStamp.Value).Ticks*currentValue;
                }
                else
                {
                    start = newRow.Key;
                }
                currentValue = newRow.Value.ToDouble();
                currentTimeStamp = newRow.Key;
            }

            if (serie.NextRow != null && serie.StopTime != null)
            {
                stop = serie.StopTime.Value;
                if (currentTimeStamp != null)
                {
                    valueSum += (stop - currentTimeStamp.Value).Ticks * currentValue;
                }
            }

            double result = valueSum / (stop - start).Ticks;

            return result.ToType<T>();
        }

        //public static float? TimeForCondition(this IQuerySerie<float> serie, Func<float, bool> )
        //{
        //    if (!serie.Rows.Any()) return null;
        //    double valueSum = 0;
        //    var rows = serie.Rows;
        //    DateTime start = DateTime.MinValue;
        //    DateTime stop = rows.Last().Key;
        //    DateTime? currentTimeStamp = null;


        //    float currentValue = 0;
        //    if (serie.PreviousRow != null && serie.StartTime != null)
        //    {
        //        start = serie.StartTime.Value;
        //        currentValue = serie.PreviousRow.Value;
        //        currentTimeStamp = start;
        //    }

        //    for (int i = 0; i < rows.Count; i++)
        //    {
        //        var newRow = rows[i];
        //        if (currentTimeStamp != null)
        //        {
        //            valueSum += (newRow.Key - currentTimeStamp.Value).Ticks * currentValue;
        //        }
        //        else
        //        {
        //            start = newRow.Key;
        //        }
        //        currentValue = newRow.Value;
        //        currentTimeStamp = newRow.Key;
        //    }

        //    if (serie.NextRow != null && serie.StopTime != null)
        //    {
        //        stop = serie.StopTime.Value;
        //        if (currentTimeStamp != null)
        //        {
        //            valueSum += (stop - currentTimeStamp.Value).Ticks * currentValue;
        //        }
        //    }

        //    var result = valueSum / (stop - start).Ticks;

        //    return (float)result;
        //}


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.ByTrigger
{
    public static class GroupByTriggerExtensions
    {
        public static IReadOnlyList<StartEndTime> GroupTimesByTrigger<T>(this IQuerySerie<T> serie, Func<IGroupTimesByTriggerConfigurator<T>, IGroupByTriggerOptional> configFunc)
           where T : struct
        {
            var rows = serie.Rows;
            List<StartEndTime> groupTimes = new List<StartEndTime>();

            if (rows.Any() == false) return groupTimes;

            var config = new GroupByTriggerConfigurator<T>();
            configFunc(config);


            bool currentlyConditionIsTrue = false;
            DateTime? startTime = serie.StartTime;

            if (serie.PreviousRow != null && config.PredicateFunc(serie.PreviousRow))
            {
                currentlyConditionIsTrue = true;
            }

            for (int i = 0; i < rows.Count; i++)
            {
                bool condition = config.PredicateFunc(rows[i]);
                if (condition && !currentlyConditionIsTrue)
                {
                    currentlyConditionIsTrue = true;
                    startTime = rows[i].Time;
                }
                if (currentlyConditionIsTrue && !condition)
                {
                    currentlyConditionIsTrue = false;
                    groupTimes.Add(CreateGroupTime(startTime.Value, rows[i].Time, config));
                }
            }

            if (currentlyConditionIsTrue)
            {
                DateTime endTime = rows.Last().Time;
                if (serie.NextRow != null && config.PredicateFunc(serie.NextRow))
                {
                    endTime = serie.EndTime??endTime;
                }
                groupTimes.Add(CreateGroupTime(startTime.Value, endTime, config));
            }
            return groupTimes;
        }

        private static StartEndTime CreateGroupTime<T>(DateTime startTime, DateTime endTime, GroupByTriggerConfigurator<T> config) where T:struct 
        {
            if (config.EndTimeIsStartTime)
            {
                endTime = startTime;
            }
            if (config.StartTimeIsEndTime)
            {
                startTime = endTime;
            }
            if (config.StartOffsetTimeSpan != null)
            {
                startTime = startTime + config.StartOffsetTimeSpan.Value;
            }
            if (config.EndOffsetTimeSpan != null)
            {
                endTime = endTime + config.EndOffsetTimeSpan.Value;
            }
            return new StartEndTime(startTime, endTime);
        }

        //public static INullableQuerySerie<T> GroupByTrigger<T>(this IQuerySerie<T> serie, Func<IGroupByTriggerConfigurator<T>, IGroupByTriggerOptional> configFunc)
        //    where T : struct
        //{
        //    var


        //}


    }
}

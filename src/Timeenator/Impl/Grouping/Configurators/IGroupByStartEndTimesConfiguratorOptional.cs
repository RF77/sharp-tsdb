using System;

namespace Timeenator.Impl.Grouping.Configurators
{
    public interface IGroupByStartEndTimesConfiguratorOptional<T> : IGroupAggregationConfigurator<T>,
        IGroupItemSelectorConfigurator<T> where T : struct
    {
        IGroupByStartEndTimesConfiguratorOptional<T> StartOffset(TimeSpan offset);
        IGroupByStartEndTimesConfiguratorOptional<T> EndOffset(TimeSpan offset);
        IGroupByStartEndTimesConfiguratorOptional<T> StartOffset(string offset);
        IGroupByStartEndTimesConfiguratorOptional<T> EndOffset(string offset);
        IGroupByStartEndTimesConfiguratorOptional<T> TimeStampIsStart();
        IGroupByStartEndTimesConfiguratorOptional<T> TimeStampIsMiddle();
        IGroupByStartEndTimesConfiguratorOptional<T> TimeStampIsEnd();
        IGroupByStartEndTimesConfiguratorOptional<T> EndIsStart();
        IGroupByStartEndTimesConfiguratorOptional<T> StartIsEnd();

        /// <summary>
        /// Expands the current time range by a specified factor
        /// e.g. is grouped by one hour and expanded by factor 5
        /// Result: time range inlcudes now the previous 2h, the grouped hour and 2h aftwerwards
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        IGroupByStartEndTimesConfiguratorOptional<T> ExpandTimeRangeByFactor(double factor);

        /// <summary>
        /// Expands the current time range by a specified time
        /// e.g. is grouped by one hour and expanded 2h
        /// Result: time range inlcudes now the previous hour, the grouped hour and 1h aftwerwards
        /// </summary>
        /// <returns></returns>
        IGroupByStartEndTimesConfiguratorOptional<T> ExpandTimeRange(TimeSpan timeSpan);

        /// <summary>
        /// Expands the current time range by a specified time
        /// e.g. is grouped by one hour and expanded 2h
        /// Result: time range inlcudes now the previous hour, the grouped hour and 1h aftwerwards
        /// </summary>

        IGroupByStartEndTimesConfiguratorOptional<T> ExpandTimeRange(string timeSpanExpression);

        /// <summary>
        /// Time between two groups must be higher than the tolareance otherwise the groups will be merged
        /// </summary>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        IGroupByStartEndTimesConfiguratorOptional<T> TimeTolerance(TimeSpan tolerance);

        /// <summary>
        /// Time between two groups must be higher than the tolareance otherwise the groups will be merged
        /// </summary>
        /// <param name="tolerance">time span as string e.g. 1s or 3d</param>
        /// <returns></returns>
        IGroupByStartEndTimesConfiguratorOptional<T> TimeTolerance(string tolerance);
    }
}
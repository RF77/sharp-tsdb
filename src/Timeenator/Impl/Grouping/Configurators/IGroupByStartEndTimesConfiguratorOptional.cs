using System;

namespace Timeenator.Impl.Grouping.Configurators
{
    public interface IGroupByStartEndTimesConfiguratorOptional<T> : IGroupAggregationConfigurator<T> where T : struct
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
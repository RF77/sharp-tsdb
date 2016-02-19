using System;

namespace Timeenator.Impl.Grouping.Configurators
{
    public interface IGroupByStartEndTimesConfigurator<T> where T : struct
    {
        IGroupByStartEndTimesConfigurator<T> StartOffset(TimeSpan offset);
        IGroupByStartEndTimesConfigurator<T> EndOffset(TimeSpan offset);
        IGroupByStartEndTimesConfigurator<T> StartOffset(string offset);
        IGroupByStartEndTimesConfigurator<T> EndOffset(string offset);
        IGroupByStartEndTimesConfigurator<T> TimeStampIsStart();
        IGroupByStartEndTimesConfigurator<T> TimeStampIsMiddle();
        IGroupByStartEndTimesConfigurator<T> TimeStampIsEnd();
        IGroupByStartEndTimesConfigurator<T> EndIsStart();
        IGroupByStartEndTimesConfigurator<T> StartIsEnd();

        /// <summary>
        /// Time between two groups must be higher than the tolareance otherwise the groups will be merged
        /// </summary>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        IGroupByStartEndTimesConfigurator<T> TimeTolerance(TimeSpan tolerance);

        /// <summary>
        /// Time between two groups must be higher than the tolareance otherwise the groups will be merged
        /// </summary>
        /// <param name="tolerance">time span as string e.g. 1s or 3d</param>
        /// <returns></returns>
        IGroupByStartEndTimesConfigurator<T> TimeTolerance(string tolerance);
    }
}
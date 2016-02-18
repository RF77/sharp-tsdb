using System;
using DbInterfaces.Interfaces;

namespace QueryLanguage.Grouping.ByTrigger
{
    public interface IGroupByTriggerConfigurator<T> where T:struct
    {
        IGroupByTriggerAggregation<T> TriggerWhen(Func<ISingleDataRow<T>, bool> predicate);
    }

    public interface IGroupTimesByTriggerConfigurator<T> where T : struct
    {
        IGroupByTriggerOptional TriggerWhen(Func<ISingleDataRow<T>, bool> predicate);
    }


    public interface IGroupByTriggerAggregation<T> where T : struct
    {
        IGroupByTriggerOptional Aggregate(Func<IQuerySerie<T>, T?> aggregationFunc);
    }

    public interface IGroupByTriggerOptional
    {
        IGroupByTriggerOptional StartOffset(TimeSpan offset);
        IGroupByTriggerOptional EndOffset(TimeSpan offset);
        IGroupByTriggerOptional StartOffset(string offset);
        IGroupByTriggerOptional EndOffset(string offset);
        IGroupByTriggerOptional TimeStampIsStart();
        IGroupByTriggerOptional TimeStampIsMiddle();
        IGroupByTriggerOptional TimeStampIsEnd();
        IGroupByTriggerOptional EndIsStart();
        IGroupByTriggerOptional StartIsEnd();

        /// <summary>
        /// Time between two groups must be higher than the tolareance otherwise the groups will be merged
        /// </summary>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        IGroupByTriggerOptional TimeTolerance(TimeSpan tolerance);

        /// <summary>
        /// Time between two groups must be higher than the tolareance otherwise the groups will be merged
        /// </summary>
        /// <param name="tolerance">time span as string e.g. 1s or 3d</param>
        /// <returns></returns>
        IGroupByTriggerOptional TimeTolerance(string tolerance);

    }
}
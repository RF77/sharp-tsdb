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
        IGroupByTriggerOptional TimeStampIsStart();
        IGroupByTriggerOptional TimeStampIsMiddle();
        IGroupByTriggerOptional TimeStampIsEnd();
        IGroupByTriggerOptional EndIsStart();
        IGroupByTriggerOptional StartIsEnd();
    }
}
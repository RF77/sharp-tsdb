using System;
using System.Collections.Generic;
using System.Linq;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    internal class GroupByTriggerConfigurator<T> : GroupByStartEndTimesConfigurator<T>, IGroupByTriggerConfigurator<T>
         where T : struct
    {
        public GroupByTriggerConfigurator(IQuerySerie<T> serie):base(serie)
        {
        }

        public Func<ISingleDataRow<T>, bool> PredicateFunc { get; set; }

        public IGroupByStartEndTimesConfiguratorOptional<T> TriggerWhen(Func<ISingleDataRow<T>, bool> predicate)
        {
            PredicateFunc = predicate;
            return this;
        }

        public override INullableQuerySerie<T> ExecuteGrouping()
        {
            var rows = Serie.Rows;
            GroupTimes = new List<StartEndTime>();

            if (rows.Any() == false) return new NullableQuerySerie<T>(new List<ISingleDataRow<T?>>(), Serie);

            CreateGroupTimes();

            return base.ExecuteGrouping();
        }

        public override IReadOnlyList<StartEndTime> CreateGroupTimes()
        {
            var rows = Serie.Rows;
            GroupTimes = new List<StartEndTime>();

            if (rows.Any() == false) return new List<StartEndTime>();

            bool currentlyConditionIsTrue = false;
            DateTime? startTime = Serie.StartTime;

            if (Serie.PreviousRow != null && PredicateFunc(Serie.PreviousRow))
            {
                currentlyConditionIsTrue = true;
            }

            for (int i = 0; i < rows.Count; i++)
            {
                bool condition = PredicateFunc(rows[i]);
                if (condition && !currentlyConditionIsTrue)
                {
                    currentlyConditionIsTrue = true;
                    startTime = rows[i].TimeUtc;
                }
                if (currentlyConditionIsTrue && !condition)
                {
                    currentlyConditionIsTrue = false;
                    GroupTimes.Add(CreateGroupTime(startTime.Value, rows[i].TimeUtc));
                }
            }

            if (currentlyConditionIsTrue)
            {
                DateTime endTime = rows.Last().TimeUtc;
                if (Serie.NextRow != null && PredicateFunc(Serie.NextRow))
                {
                    endTime = Serie.EndTime ?? endTime;
                }
                GroupTimes.Add(CreateGroupTime(startTime.Value, endTime));
            }
            return GroupTimes;
        }
    }
}
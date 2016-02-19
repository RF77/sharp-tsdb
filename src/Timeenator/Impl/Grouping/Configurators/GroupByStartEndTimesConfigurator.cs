using System;
using System.Collections.Generic;
using System.Linq;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    internal class GroupByStartEndTimesConfigurator<T>: GroupConfigurator<T>, IGroupByStartEndTimesConfigurator<T>, IGroupByStartEndTimesConfiguratorOptional<T> where T:struct
    {
        public TimeSpan? StartOffsetTimeSpan { get; set; }
        public TimeSpan? EndOffsetTimeSpan { get; set; }
        public TimeStampType TimeStampType { get; set; }
        public bool EndTimeIsStartTime { get; set; }
        public bool StartTimeIsEndTime { get; set; }
        public TimeSpan? Tolerance { get; set; }

        public GroupByStartEndTimesConfigurator(IQuerySerie<T> serie):base(serie)
        {
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> StartOffset(TimeSpan offset)
        {
            StartOffsetTimeSpan = offset;
            return this;
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> EndOffset(TimeSpan offset)
        {
            EndOffsetTimeSpan = offset;
            return this;
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> StartOffset(string offset)
        {
            return StartOffset(offset.ToTimeSpan());
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> EndOffset(string offset)
        {
            return EndOffset(offset.ToTimeSpan());
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> TimeStampIsStart()
        {
            TimeStampType = TimeStampType.Start;
            return this;
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> TimeStampIsMiddle()
        {
            TimeStampType = TimeStampType.Start;
            return this;
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> TimeStampIsEnd()
        {
            TimeStampType = TimeStampType.End;
            return this;
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> EndIsStart()
        {
            EndTimeIsStartTime = true;
            return this;
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> StartIsEnd()
        {
            StartTimeIsEndTime = true;
            return this;
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> TimeTolerance(TimeSpan tolerance)
        {
            Tolerance = tolerance;
            return this;
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> TimeTolerance(string tolerance)
        {
            return TimeTolerance(tolerance.ToTimeSpan());
        }

        public override INullableQuerySerie<T> ExecuteGrouping()
        {
            var rows = Serie.Rows;
            if (!rows.Any() || !GroupTimes.Any())
                return new NullableQuerySerie<T>(new List<ISingleDataRow<T?>>(), Serie);

            int index = 0;
            int max = rows.Count;

            List<ISingleDataRow<T?>> result = new List<ISingleDataRow<T?>>();

            foreach (var groupTime in GroupTimes)
            {
                List<ISingleDataRow<T>> group = new List<ISingleDataRow<T>>();
                while (index < max && rows[index].Time < groupTime.Start)
                {
                    index++;
                }
                var startIndex = index;

                while (index < max && rows[index].Time < groupTime.End)
                {
                    group.Add(rows[index]);

                    index++;
                }
                var aggregationData = new QuerySerie<T>(@group, groupTime.Start, groupTime.End)
                {
                    PreviousRow = startIndex > 0 ? rows[startIndex - 1] : Serie.PreviousRow,
                    NextRow = index < max ? rows[index] : Serie.NextRow
                };
                result.Add(new SingleDataRow<T?>(groupTime.GetTimeStampByType(TimeStampType),
                    AggregationFunc(aggregationData)));
            }

            var resultData = new NullableQuerySerie<T>(result, Serie);
            return resultData;
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> ByTimeRanges(IReadOnlyList<StartEndTime> groupTimes)
        {
            GroupTimes = groupTimes;
            return this;
        }

        protected IReadOnlyList<StartEndTime> GroupTimes { get; set; }
    }
}
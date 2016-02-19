using System;
using System.Collections.Generic;
using System.Linq;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    internal class GroupByStartEndTimesConfigurator<T> : GroupConfigurator<T>, IGroupByStartEndTimesConfigurator<T>,
        IGroupByStartEndTimesConfiguratorOptional<T>, IGroupTimesCreator where T : struct
    {
        public GroupByStartEndTimesConfigurator(IQuerySerie<T> serie) : base(serie)
        {
        }

        public TimeSpan? StartOffsetTimeSpan { get; set; }
        public TimeSpan? EndOffsetTimeSpan { get; set; }
        public TimeStampType TimeStampType { get; set; }
        public bool EndTimeIsStartTime { get; set; }
        public bool StartTimeIsEndTime { get; set; }
        public TimeSpan? Tolerance { get; set; }
        protected List<StartEndTime> GroupTimes { get; set; }

        public IGroupByStartEndTimesConfiguratorOptional<T> ByTimeRanges(IReadOnlyList<StartEndTime> groupTimes)
        {
            GroupTimes = (groupTimes as List<StartEndTime>) ?? groupTimes.ToList();
            return this;
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

            var index = 0;
            var max = rows.Count;

            var result = new List<ISingleDataRow<T?>>();

            foreach (var groupTime in GroupTimes)
            {
                var group = new List<ISingleDataRow<T>>();
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

        protected StartEndTime CreateGroupTime(DateTime startTime, DateTime endTime)
        {
            if (EndTimeIsStartTime)
            {
                endTime = startTime;
            }
            if (StartTimeIsEndTime)
            {
                startTime = endTime;
            }
            if (StartOffsetTimeSpan != null)
            {
                startTime = startTime + StartOffsetTimeSpan.Value;
            }
            if (EndOffsetTimeSpan != null)
            {
                endTime = endTime + EndOffsetTimeSpan.Value;
            }
            return new StartEndTime(startTime, endTime);
        }

        public virtual IReadOnlyList<StartEndTime> CreateGroupTimes()
        {
            return GroupTimes;
        }
    }
}
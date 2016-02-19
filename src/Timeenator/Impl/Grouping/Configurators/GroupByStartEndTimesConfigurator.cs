using System;

namespace Timeenator.Impl.Grouping.Configurators
{
    internal abstract class GroupByStartEndTimesConfigurator<T>: GroupConfigurator<T>, IGroupByStartEndTimesConfigurator<T> where T:struct
    {
        public TimeSpan? StartOffsetTimeSpan { get; set; }
        public TimeSpan? EndOffsetTimeSpan { get; set; }
        public TimeStampType TimeStampType { get; set; }
        public bool EndTimeIsStartTime { get; set; }
        public bool StartTimeIsEndTime { get; set; }
        public TimeSpan? Tolerance { get; set; }

        public IGroupByStartEndTimesConfigurator<T> StartOffset(TimeSpan offset)
        {
            StartOffsetTimeSpan = offset;
            return this;
        }

        public IGroupByStartEndTimesConfigurator<T> EndOffset(TimeSpan offset)
        {
            EndOffsetTimeSpan = offset;
            return this;
        }

        public IGroupByStartEndTimesConfigurator<T> StartOffset(string offset)
        {
            return StartOffset(offset.ToTimeSpan());
        }

        public IGroupByStartEndTimesConfigurator<T> EndOffset(string offset)
        {
            return EndOffset(offset.ToTimeSpan());
        }

        public IGroupByStartEndTimesConfigurator<T> TimeStampIsStart()
        {
            TimeStampType = TimeStampType.Start;
            return this;
        }

        public IGroupByStartEndTimesConfigurator<T> TimeStampIsMiddle()
        {
            TimeStampType = TimeStampType.Start;
            return this;
        }

        public IGroupByStartEndTimesConfigurator<T> TimeStampIsEnd()
        {
            TimeStampType = TimeStampType.End;
            return this;
        }

        public IGroupByStartEndTimesConfigurator<T> EndIsStart()
        {
            EndTimeIsStartTime = true;
            return this;
        }

        public IGroupByStartEndTimesConfigurator<T> StartIsEnd()
        {
            StartTimeIsEndTime = true;
            return this;
        }

        public IGroupByStartEndTimesConfigurator<T> TimeTolerance(TimeSpan tolerance)
        {
            Tolerance = tolerance;
            return this;
        }

        public IGroupByStartEndTimesConfigurator<T> TimeTolerance(string tolerance)
        {
            return TimeTolerance(tolerance.ToTimeSpan());
        }
    }
}
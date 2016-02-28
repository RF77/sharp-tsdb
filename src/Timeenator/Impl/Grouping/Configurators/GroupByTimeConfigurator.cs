using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping.Configurators
{
    public class GroupByTimeConfigurator<T> : GroupByStartEndTimesConfigurator<T>, IGroupByTimeConfigurator<T> where T : struct
    {
        public IGroupByStartEndTimesConfiguratorOptional<T> Expression(string expression, string minimalExpression = null)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            if (minimalExpression != null)
            {
                var minTimeSpan = minimalExpression.ToTimeSpan();
                var currentTimeSpan = expression.ToTimeSpan();
                if (currentTimeSpan < minTimeSpan)
                {
                    expression = minimalExpression;
                }
            }
            expression = expression.Trim();
            var match = Regex.Match(expression, "^(\\d+)([smhdwMy])$");
            if (match.Success == false)
            {
                throw new ArgumentException($"expression {expression} is invalid");
            }
            int number = int.Parse(match.Groups[1].Value);
            string type = match.Groups[2].Value;

            switch (type)
            {
                case "s":
                    Seconds(number);
                    break;
                case "m":
                    Minutes(number);
                    break;
                case "h":
                     Hours(number);
                    break;
                case "d":
                    Days(number);
                    break;
                case "w":
                    Weeks(number);
                    break;
                case "M":
                    Months(number);
                    break;
                case "y":
                    Years(number);
                    break;
                default:
                    throw new ArgumentException($"expression {expression} has unknown type");
            }
            return this;
        }

        public GroupByTimeConfigurator(IQuerySerie<T> serie) : base(serie)
        {

        }

        public IGroupByStartEndTimesConfiguratorOptional<T> Seconds(int seconds)
        {
            if (!Serie.Rows.Any()) return this;
            ISingleDataRow<T> first = Serie.Rows.First();
            DateTime d = Serie.StartTime ?? first.Time;

            int startSeconds = d.Second;
            if (60 % seconds == 0)
            {
                //change start minute to even minute
                startSeconds = startSeconds - (startSeconds % seconds);
            }

            DateTime currentDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, startSeconds);

            return GroupByTime(currentDate, Serie.EndTime, dt => dt + TimeSpan.FromSeconds(seconds));
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> Minutes(int minutes)
        {
            if (!Serie.Rows.Any()) return this;
            ISingleDataRow<T> first = Serie.Rows.First();
            DateTime d = Serie.StartTime ?? first.Time;

            int startMinute = d.Minute;
            if (60 % minutes == 0)
            {
                //change start minute to even minute
                startMinute = startMinute - (startMinute % minutes);
            }

            DateTime currentDate = new DateTime(d.Year, d.Month, d.Day, d.Hour, startMinute, 0);

            return GroupByTime(currentDate, Serie.EndTime, dt => dt + TimeSpan.FromMinutes(minutes));
        }
        public IGroupByStartEndTimesConfiguratorOptional<T> Hours(int hours)
        {
            if (!Serie.Rows.Any()) return this;
            ISingleDataRow<T> first = Serie.Rows.First();
            DateTime d = Serie.StartTime ?? first.Time;

            int startHour = d.Hour;
            if (24 % hours == 0)
            {
                //change start minute to even minute
                startHour = startHour - (startHour % hours);
            }

            DateTime currentDate = new DateTime(d.Year, d.Month, d.Day, startHour, 0, 0);

            return GroupByTime(currentDate, Serie.EndTime, dt => dt + TimeSpan.FromHours(hours));
        }
        public IGroupByStartEndTimesConfiguratorOptional<T> Days(int days, int startHour)
        {
            if (!Serie.Rows.Any()) return this;
            ISingleDataRow<T> first = Serie.Rows.First();
            DateTime d = Serie.StartTime ?? first.Time;

            DateTime currentDate = new DateTime(d.Year, d.Month, d.Day, startHour, 0, 0);

            return GroupByTime(currentDate, Serie.EndTime, dt => dt + TimeSpan.FromDays(days));
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> Days(int days)
        {
            return Days(days, 0);
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> Weeks(int weeks, DayOfWeek startDay = DayOfWeek.Monday)
        {
            if (!Serie.Rows.Any()) return this;
            ISingleDataRow<T> first = Serie.Rows.First();
            DateTime d = Serie.StartTime ?? first.Time;

            DateTime startDate = new DateTime(d.Year, d.Month, d.Day);

            while (startDate.DayOfWeek != startDay)
            {
                startDate = startDate - TimeSpan.FromDays(1);
            }

            return GroupByTime(startDate, Serie.EndTime, dt => dt + TimeSpan.FromDays(weeks * 7));
        }
        public IGroupByStartEndTimesConfiguratorOptional<T> Months(int months)
        {
            if (!Serie.Rows.Any()) return this;
            ISingleDataRow<T> first = Serie.Rows.First();
            DateTime d = Serie.StartTime ?? first.Time;

            int startMonth = d.Month;
            if (12 % months == 0)
            {
                startMonth = startMonth - (startMonth % months) + 1;
            }

            DateTime currentDate = new DateTime(d.Year, startMonth, 1, 0, 0, 0);

            return GroupByTime(currentDate, Serie.EndTime, dt => 
            {
                int year = dt.Year;
                int month = dt.Month;
                int newMonth = month + months;
                year += (newMonth - 1) / 12;
                month = ((newMonth - 1) % 12) + 1;
                return new DateTime(year, month, 1);
            });
        }
        public IGroupByStartEndTimesConfiguratorOptional<T> Years(int years)
        {
            if (!Serie.Rows.Any()) return this;
            ISingleDataRow<T> first = Serie.Rows.First();
            DateTime d = Serie.StartTime ?? first.Time;

            DateTime currentDate = new DateTime(d.Year, 1, 1);

            return GroupByTime(currentDate, Serie.EndTime, dt => new DateTime(dt.Year + years, 1, 1));
        }

        public IGroupByStartEndTimesConfiguratorOptional<T> GroupByTime(DateTime startTime, DateTime? stopTime, Func<DateTime, DateTime> calcNewDateMethod)
            
        {
            List<StartEndTime> result = new List<StartEndTime>();
            DateTime endTime = calcNewDateMethod(startTime);

            DateTime lastTime = stopTime ?? Serie.Rows.Last().Time;

            while (startTime < lastTime)
            {
                result.Add(new StartEndTime(startTime, endTime));
                startTime = endTime;
                endTime = calcNewDateMethod(startTime);
            }

            GroupTimes = result;

            return this;
        }

        public override INullableQuerySerie<T> ExecuteGrouping()
        {
            CreateGroupTimes(); //used to assigne optional time range
            return base.ExecuteGrouping();
        }
    }
}
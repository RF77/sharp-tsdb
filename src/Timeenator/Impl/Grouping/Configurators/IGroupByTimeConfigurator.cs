using System;
using System.Collections.Generic;

namespace Timeenator.Impl.Grouping.Configurators
{
    public interface IGroupByTimeConfigurator<T> where T : struct
    {
        IGroupByStartEndTimesConfiguratorOptional<T> Expression(string expression);
        IGroupByStartEndTimesConfiguratorOptional<T> Expression(string expression, string minimalExpression);
        IGroupByStartEndTimesConfiguratorOptional<T> Span(TimeSpan timeSpan);
        IGroupByStartEndTimesConfiguratorOptional<T> Span(TimeSpan timeSpan, TimeSpan? minTimeSpan);
        IGroupByStartEndTimesConfiguratorOptional<T> Seconds(int seconds);
        IGroupByStartEndTimesConfiguratorOptional<T> Minutes(int minutes);
        IGroupByStartEndTimesConfiguratorOptional<T> Hours(int hours);
        IGroupByStartEndTimesConfiguratorOptional<T> Days(int days, int startHour);
        IGroupByStartEndTimesConfiguratorOptional<T> Days(int days);
        IGroupByStartEndTimesConfiguratorOptional<T> Weeks(int weeks, DayOfWeek startDay = DayOfWeek.Monday);
        IGroupByStartEndTimesConfiguratorOptional<T> Months(int months);
        IGroupByStartEndTimesConfiguratorOptional<T> Years(int years);
    }
}
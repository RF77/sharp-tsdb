using System;
using System.Collections.Generic;

namespace Timeenator.Impl.Grouping.Configurators
{
    public interface IGroupByTimeConfigurator<T> where T : struct
    {
        IGroupByStartEndTimesConfiguratorOptional<T> Expression(string expression, string minimalExpression = null);
        IGroupByStartEndTimesConfiguratorOptional<T> Seconds(int seconds);
        IGroupByStartEndTimesConfiguratorOptional<T> Minutes(int minutes);
        IGroupByStartEndTimesConfiguratorOptional<T> Hours(int hours);
        IGroupByStartEndTimesConfiguratorOptional<T> Days(int days, int startHour = 0);
        IGroupByStartEndTimesConfiguratorOptional<T> Weeks(int weeks, DayOfWeek startDay = DayOfWeek.Monday);
        IGroupByStartEndTimesConfiguratorOptional<T> Months(int months);
        IGroupByStartEndTimesConfiguratorOptional<T> Years(int years);
    }
}
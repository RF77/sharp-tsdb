// /*******************************************************************************
//  * Copyright (c) 2016 by RF77 (https://github.com/RF77)
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the Eclipse Public License v1.0
//  * which accompanies this distribution, and is available at
//  * http://www.eclipse.org/legal/epl-v10.html
//  *
//  * Contributors:
//  *    RF77 - initial API and implementation and/or initial documentation
//  *******************************************************************************/ 

using System;

namespace Timeenator.Impl.Grouping.Configurators
{
    public interface IGroupByTimeConfigurator<T> where T : struct
    {
        IGroupByStartEndTimesConfiguratorOptional<T> Expression(string expression);
        IGroupByStartEndTimesConfiguratorOptional<T> Expression(string expression, string minimalExpression);
        IGroupByStartEndTimesConfiguratorOptional<T> Expression(string expression, double factor);
        IGroupByStartEndTimesConfiguratorOptional<T> Expression(string expression, string minimalExpression, double factor);

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
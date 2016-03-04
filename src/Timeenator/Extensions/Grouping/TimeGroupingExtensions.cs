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
using Timeenator.Interfaces;

namespace Timeenator.Extensions.Grouping
{
    public static class TimeGroupingExtensions
    {
        public static INullableQuerySerie<T> GroupBy<T>(this IQuerySerie<T> serie, string expression,
            Func<IQuerySerie<T>, T?> aggregationFunc) where T : struct
        {
            return serie.Group(g => g.ByTime.Expression(expression).Aggregate(aggregationFunc));
        }

        public static INullableQuerySerie<T> GroupBySeconds<T>(this IQuerySerie<T> serie, int seconds,
            Func<IQuerySerie<T>, T?> aggregationFunc) where T : struct
        {
            return serie.Group(g => g.ByTime.Seconds(seconds).Aggregate(aggregationFunc));
        }

        public static INullableQuerySerie<T> GroupByMinutes<T>(this IQuerySerie<T> serie, int minutes,
            Func<IQuerySerie<T>, T?> aggregationFunc) where T : struct
        {
            return serie.Group(g => g.ByTime.Minutes(minutes).Aggregate(aggregationFunc));
        }

        public static INullableQuerySerie<T> GroupByHours<T>(this IQuerySerie<T> serie, int hours,
            Func<IQuerySerie<T>, T?> aggregationFunc) where T : struct
        {
            return serie.Group(g => g.ByTime.Hours(hours).Aggregate(aggregationFunc));
        }

        public static INullableQuerySerie<T> GroupByDays<T>(this IQuerySerie<T> serie, int days,
            Func<IQuerySerie<T>, T?> aggregationFunc) where T : struct
        {
            return serie.Group(g => g.ByTime.Days(days).Aggregate(aggregationFunc));
        }
    }
}
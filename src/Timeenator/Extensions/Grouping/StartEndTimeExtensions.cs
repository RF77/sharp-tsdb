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
using System.Collections.Generic;
using System.Linq;
using Timeenator.Impl.Grouping;

namespace Timeenator.Extensions.Grouping
{
    public static class StartEndTimeExtensions
    {
        public static IReadOnlyList<StartEndTime> CombineByTolerance(this IReadOnlyList<StartEndTime> groups,
            TimeSpan tolerance)
        {
            var newGroups = new List<StartEndTime>(groups.Count);
            var previous = groups.First();
            for (var i = 1; i < groups.Count; i++)
            {
                var current = groups[i];
                if ((current.Start - previous.End) <= tolerance)
                {
                    previous = new StartEndTime(previous.Start, current.End > previous.End ? current.End : previous.End);
                }
                else
                {
                    newGroups.Add(previous);
                    previous = current;
                }
            }
            newGroups.Add(previous);

            return newGroups;
        }

        public static IReadOnlyList<StartEndTime> CombineByTolerance(this IReadOnlyList<StartEndTime> groups,
            string tolerance)
        {
            return groups.CombineByTolerance(tolerance.ToTimeSpan());
        }
    }
}
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
using System.Diagnostics;

namespace Timeenator.Impl.Grouping
{
    [DebuggerDisplay("{Start} - {End}: {Duration}")]
    public class StartEndTime
    {
        public StartEndTime(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan Duration => End - Start;

        public DateTime GetTimeStampByType(TimeStampType timeStampType = TimeStampType.Start)
        {
            if (timeStampType == TimeStampType.Start) return Start;
            if (timeStampType == TimeStampType.End)
            {
                return End;
            }
            if (timeStampType == TimeStampType.Middle)
            {
                return Start + TimeSpan.FromMinutes((End - Start).TotalMinutes/2);
            }
            return Start;
        }
    }
}
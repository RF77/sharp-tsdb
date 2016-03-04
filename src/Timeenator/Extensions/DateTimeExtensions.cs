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

namespace Timeenator.Extensions
{
    public static class DateTimeExtensions
    {
        public static uint ToSecondsAfter1970Utc(this DateTime date)
        {
            return (uint) (date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public static long ToMiliSecondsAfter1970(this DateTime date)
        {
            return
                (long) (date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public static long ToMiliSecondsAfter1970Utc(this DateTime date)
        {
            return (long) (date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public static long? ToFileTimeUtc(this DateTime? date)
        {
            if (date == null) return null;
            return date.Value.ToFileTimeUtc();
        }
    }
}
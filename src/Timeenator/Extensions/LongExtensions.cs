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
    public static class LongExtensions
    {
        public static DateTime FromSecondsAfter1970ToDateTimeUtc(this long seconds)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromSeconds(seconds));
        }

        public static DateTime FromMilisecondsAfter1970ToDateTimeUtc(this long seconds)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(seconds));
        }

        public static DateTime? FromFileTimeUtcToDateTimeUtc(this long? fileTime)
        {
            if (fileTime == null) return null;
            return DateTime.FromFileTimeUtc(fileTime.Value);
        }
    }
}
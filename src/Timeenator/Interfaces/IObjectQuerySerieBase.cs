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

namespace Timeenator.Interfaces
{
    public interface IObjectQuerySerieBase : IQueryResult
    {
        DateTime? StartTime { get; }
        DateTime? EndTime { get; }
        string Name { get; set; }
        string GroupName { get; set; }
        string Key { get; set; }
        string OriginalName { get; }
        string FullName { get; }

        /// <summary>
        ///     Last value before the start time or null
        /// </summary>
        IObjectSingleDataRow PreviousRow { get; }

        /// <summary>
        ///     first value after end time or null
        /// </summary>
        IObjectSingleDataRow NextRow { get; }

        /// <summary>
        ///     defaults to null, but can be explicitly set to show in a chart the current value
        /// </summary>
        IObjectSingleDataRow LastRow { get; }

        object this[int index] { get; set; }
    }
}
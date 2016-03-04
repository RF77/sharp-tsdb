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

namespace Timeenator.Interfaces
{
    public interface IQuerySerieBase<T> : IObjectQuerySerieBase where T : struct
    {
        /// <summary>
        ///     Last value before the start time or null
        /// </summary>
        new ISingleDataRow<T> PreviousRow { get; set; }

        /// <summary>
        ///     first value after end time or null
        /// </summary>
        new ISingleDataRow<T> NextRow { get; set; }

        /// <summary>
        ///     defaults to null, but can be explicitly set to show in a chart the current value
        /// </summary>
        new ISingleDataRow<T> LastRow { get; set; }
    }
}
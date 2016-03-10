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

namespace Timeenator.Impl
{
    public abstract class QuerySerieBase<T> : ObjectQuerySerie, IQuerySerieBase<T> where T : struct
    {
        protected QuerySerieBase(DateTime? startTime, DateTime? endTime):base(startTime, endTime)
        {
        }

        protected QuerySerieBase(IQuerySerieBase<T> serie):base(serie)
        {
        }

        public new ISingleDataRow<T> PreviousRow
        {
            get { return base.PreviousRow as ISingleDataRow<T>; }
            set { base.PreviousRow = value; }
        }

        public new ISingleDataRow<T> NextRow
        {
            get { return base.NextRow as ISingleDataRow<T>; }
            set { base.NextRow = value; }
        }

        public new ISingleDataRow<T> LastRow
        {
            get { return base.LastRow as ISingleDataRow<T>; }
            set { base.LastRow = value; }
        }
    }
}
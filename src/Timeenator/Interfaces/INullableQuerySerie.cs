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
using Timeenator.Impl.Grouping;

namespace Timeenator.Interfaces
{
    public interface INullableQuerySerie<T> : IObjectQuerySerie, IQuerySerieBase<T> where T : struct
    {
        new IReadOnlyList<ISingleDataRow<T?>> Rows { get; }
        INullableQuerySerie<T> Clone(string serieName);

        INullableQuerySerie<T> Zip(INullableQuerySerie<T> secondQuery, string resultQueryName,
            Func<T?, T?, T?> transformAction);

        INullableQuerySerie<T> Alias(string name);
        INullableQuerySerie<T> Transform(Func<T?, T?> transformFunc);
        INullableQuerySerie<T> CalcValue(Func<T?, T?> calculationFunc, string newSerieName = null);
        INullableQuerySerie<T> FillValue(T fillValue);
        IQuerySerie<T> RemoveNulls();
        INullableQuerySerie<T> Fill(ValueForNull fillValue);
        INullableQuerySerie<T> AppendName(string name);
    }
}
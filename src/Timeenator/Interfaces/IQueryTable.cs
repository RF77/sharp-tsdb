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
using Timeenator.Impl.Grouping.Configurators;

namespace Timeenator.Interfaces
{
    public interface IQueryTable<T> : IQueryTableBase<T> where T : struct
    {
        new IEnumerable<IQuerySerie<T>> Series { get; }
        new IQuerySerie<T> TryGetSerie(string name);
        void AddSerie(IQuerySerie<T> serie);
        INullableQueryTable<T> Transform(Func<IQuerySerie<T>, INullableQuerySerie<T>> doFunc);
        INullableQueryTable<T> Group(Func<IGroupSelector<T>, IExecutableGroup<T>> groupConfigurator);
    }
}
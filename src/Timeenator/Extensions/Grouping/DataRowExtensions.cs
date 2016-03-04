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

using Timeenator.Impl.DataRows;
using Timeenator.Interfaces;

namespace Timeenator.Extensions.Grouping
{
    public static class DataRowExtensions
    {
        public static NamedDataRow<T> Named<T>(this ISingleDataRow<T> rows, string name) where T : struct
        {
            return new NamedDataRow<T>(name, rows);
        }
    }
}
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

using Timeenator.Interfaces;

namespace Timeenator.Impl.DataRows
{
    public class NamedDataRow<T> where T : struct
    {
        public NamedDataRow(string name, ISingleDataRow<T> rows)
        {
            Name = name;
            Rows = rows;
        }

        public string Name { get; set; }
        public ISingleDataRow<T> Rows { get; set; }
    }
}
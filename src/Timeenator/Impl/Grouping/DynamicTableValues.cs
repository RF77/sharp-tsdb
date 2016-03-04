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

using System.Dynamic;
using Timeenator.Interfaces;

namespace Timeenator.Impl.Grouping
{
    public class DynamicTableValues : DynamicObject
    {
        private readonly IObjectQueryTable _table;

        public DynamicTableValues(IObjectQueryTable table)
        {
            _table = table;
        }

        public int Index { get; set; }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var serie = _table.TryGetSerie(binder.Name);
            if (serie != null)
            {
                serie[Index] = value;
                return true;
            }
            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var serie = _table.TryGetSerie(binder.Name);
            if (serie != null)
            {
                result = serie[Index];
                return true;
            }
            result = null;
            return false;
        }
    }
}
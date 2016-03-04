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
    public class DynamicTable : DynamicObject
    {
        private readonly IObjectQueryTable _table;

        public DynamicTable(IObjectQueryTable table)
        {
            _table = table;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var serie = _table.TryGetSerie(binder.Name);
            if (serie != null)
            {
                result = serie;
                return true;
            }
            result = null;
            return false;
        }
    }
}
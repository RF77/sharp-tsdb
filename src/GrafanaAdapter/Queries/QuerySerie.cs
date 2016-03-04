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

using System.Collections.Generic;

namespace GrafanaAdapter.Queries
{
    public class QuerySerie
    {
        public string name { get; set; }
        public List<string> columns { get; set; } = new List<string>();
        public List<List<object>> values { get; set; } = new List<List<object>>();
    }
}
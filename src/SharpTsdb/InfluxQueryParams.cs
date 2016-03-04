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

namespace SharpTsdb
{
    public class InfluxQueryParams
    {
        public string db { get; set; }
        public string epoch { get; set; }
        public string p { get; set; }
        public string u { get; set; }
        public string q { get; set; }
    }
}
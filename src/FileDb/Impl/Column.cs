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
using System.Runtime.Serialization;
using DbInterfaces.Interfaces;

namespace FileDb.Impl
{
    [DataContract]
    public class Column : IColumn
    {
        public Column(string name, Type valueType)
        {
            Name = name;
            ValueType = valueType;
            Size = ValidTypes.SizeForType(valueType);
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Type ValueType { get; set; }

        [DataMember]
        public byte Size { get; set; }
    }
}
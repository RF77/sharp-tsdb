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
using System.Runtime.Serialization;
using DbInterfaces.Interfaces;

namespace FileDb.Impl
{
    [DataContract]
    public class MeasurementMetadata : IMeasurementMetadata
    {
        /// <summary>
        ///     Only for deserialization
        /// </summary>
        private MeasurementMetadata()
        {
        }

        public MeasurementMetadata(string name)
        {
            Name = name;
            Id = Guid.NewGuid().ToString().Replace("-", "");
        }

        [DataMember]
        public List<Column> ColumnsInternal { get; } = new List<Column>();

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public HashSet<string> Aliases { get; } = new HashSet<string>();

        [DataMember]
        public HashSet<string> Tags { get; } = new HashSet<string>();

        public IEnumerable<IColumn> Columns => ColumnsInternal;

        [DataMember]
        public string Id { get; set; }
    }
}
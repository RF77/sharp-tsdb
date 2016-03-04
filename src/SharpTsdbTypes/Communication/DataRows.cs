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
using System.Linq;
using System.Runtime.Serialization;
using Timeenator.Extensions.Converting;
using Timeenator.Impl;
using Timeenator.Interfaces;

namespace SharpTsdbTypes.Communication
{
    [DataContract]
    public class DataRows
    {
        public DataRows()
        {
        }

        public DataRows(IEnumerable<IDataRow> rows)
        {
            Rows = rows.Select(i => new[] {i.Key.ToFileTimeUtc(), i.Value}).ToList();
        }

        [DataMember]
        public List<object[]> Rows { get; set; }

        public IEnumerable<ISingleDataRow<T>> AsTyped<T>() where T : struct
        {
            return Rows?.Select(i => new SingleDataRow<T>(ToDateTime(i), i[1].ToType<T>()));
        }

        private static DateTime ToDateTime(object[] i)
        {
            return DateTime.FromFileTimeUtc((long) i[0]);
        }

        public IEnumerable<ISingleDataRow<T?>> AsNullableTyped<T>() where T : struct
        {
            return Rows?.Select(i => new SingleDataRow<T?>(ToDateTime(i), i[1]?.ToType<T>()));
        }

        public IEnumerable<IDataRow> AsIDataRows()
        {
            return Rows?.Select(i => new DataRow {Key = ToDateTime(i), Value = i[1]});
        }
    }
}